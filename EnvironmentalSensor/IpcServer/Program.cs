using EnvironmentalSensor;
using EnvironmentalSensor.Ipc;
using EnvironmentalSensor.Usb;
using EnvironmentalSensor.Usb.Payloads;
using Ksnm.ExtensionMethods.System.Collections.Generic.Enumerable;
using Ksnm.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EnvironmentalSensor.Usb.Utility;

namespace IpcServer
{
    /// <summary>
    /// サーバープログラム
    /// </summary>
    class Program
    {
        const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";

        const string ProhibitionOfMultipleActivationMutexName = "Local/EnvironmentalSensor.IpcServer";
        static Mutex ProhibitionOfMultipleActivationMutex;

        enum Mode
        {
            /// <summary>
            /// 通常
            /// </summary>
            Normal,
            /// <summary>
            /// センサーを使用せず、Dummyデータを出力する。
            /// </summary>
            Dummy,
        }

        #region 通信関係
        static SerialPort serialPort = new SerialPort();
        /// <summary>
        /// 受信データを蓄積するバッファ
        /// </summary>
        static List<byte> serialPortReceivedBuffer = new List<byte>(serialPort.ReadBufferSize);

        static Server server;
        #endregion 通信関係

        #region ログ関係
        static CsvLogger csvLogger = null;

        [Flags]
        enum LogFlags : uint
        {
            Error = 0x0000_0001,
            LatestDataLong = 0x0000_0002,
            DamagedData = 0x8000_0000,
            All = 0xFFFF_FFFF,
        }
        static LogFlags logMode = LogFlags.LatestDataLong | LogFlags.Error;
        #endregion ログ関係

        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine($"IpcServer DEBUG mode");
            // 各種テスト
            RemoveToNextHeaderTest();
#else
            Console.WriteLine($"IpcServer");
#endif
            // モード設定
            var mode = Mode.Normal;
            Console.WriteLine($"mode:{mode}");
#if false
            Console.WriteLine($"{nameof(LifetimeServices.LeaseManagerPollTime)}={LifetimeServices.LeaseManagerPollTime}");
            Console.WriteLine($"{nameof(LifetimeServices.LeaseTime)}={LifetimeServices.LeaseTime}");
            Console.WriteLine($"{nameof(LifetimeServices.RenewOnCallTime)}={LifetimeServices.RenewOnCallTime}");
            Console.WriteLine($"{nameof(LifetimeServices.SponsorshipTimeout)}={LifetimeServices.SponsorshipTimeout}");
#endif
            // 多重を禁止する処理
            if (Main_ProhibitionOfMultipleActivation() == false)
            {
                // 終了
                return;
            }

            // リース期間を無限に設定(ゼロで無限になる)
            LifetimeServices.LeaseTime = TimeSpan.Zero;
            server = new Server();

            // ポート処理は通常モードのときだけ行う
            if (mode == Mode.Normal)
            {
                var portName = "";
                while (true)
                {
                    Console.WriteLine($"ポート名を入力[COM1など]");
                    portName = Console.ReadLine();
                    // シリアルポートを設定して通信を開始
                    try
                    {
                        serialPort.ErrorReceived += SerialPort_ErrorReceived;
                        serialPort.PinChanged += SerialPort_PinChanged;
                        serialPort.DataReceived += SerialPort_DataReceived;
                        SettingSerialPort(serialPort);
                        serialPort.PortName = portName;
                        serialPort.Open();
                        // 正常終了
                        break;
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine($"終了:Q / 再入力:その他");
                        var read = Console.ReadLine();
                        if (read.ToUpper() == "Q")
                        {
                            return;
                        }
                    }
                }
            }

            // ルーティーン開始
            if (mode == Mode.Normal)
            {
                NormalRoutine();
            }
            else if (mode == Mode.Dummy)
            {
                DummyRoutine();
            }
            // 終了
            serialPort.Close();
        }
        static void NormalRoutine()
        {
            // ログ準備
            {
                var logFilePath = $"Logs/{DateTime.Now.ToString("yyyyMMdd")}.csv";
                var exists = File.Exists(logFilePath);
                csvLogger = new CsvLogger(logFilePath);
                // ログファイルがもともと無かったらヘッダーを書き込み
                if (exists == false)
                {
                    LogHeader();
                }
            }
            while (true)
            {
                // 非同期処理をCancelするためのTokenを取得.
                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    EnvironmentalSensorCommunication(cancellationTokenSource.Token);
                    // 入力待ち
                    Console.WriteLine("停止するには何かのキーを入力");
                    Console.ReadLine();
                    // 入力されたら停止
                    cancellationTokenSource.Cancel();
                }
                // 入力待ち
                Console.WriteLine($"終了する場合は[Q] それ以外のキーは再開");
                var read = Console.ReadLine();
                if (read.ToUpper() == "Q")
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 
        /// EnvironmentalSensorCommunication使わずこの関数内で
        /// LatestDataLongResponsePayloadを作成
        /// </summary>
        static void DummyRoutine()
        {
            while (true)
            {
                // 非同期処理をCancelするためのTokenを取得.
                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    DummyEnvironmentalSensorCommunication(cancellationTokenSource.Token);
                    // 入力待ち
                    Console.WriteLine("停止するには何かのキーを入力");
                    Console.ReadLine();
                    // 入力されたら停止
                    cancellationTokenSource.Cancel();
                }
                // 入力待ち
                Console.WriteLine($"終了する場合は[Q] それ以外のキーは再開");
                var read = Console.ReadLine();
                if (read.ToUpper() == "Q")
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 定期的にデータを取得
        /// </summary>
        static async void DummyEnvironmentalSensorCommunication(CancellationToken cancelToken)
        {
            var random = new Ksnm.Randoms.Xorshift128();
            var payload = new LatestDataLongResponsePayload(0);
            while (true)
            {
                // キャンセルリクエストが来たらキャンセル
                if (cancelToken.IsCancellationRequested)
                {
                    break;
                }
                // ダミー
                {
                    // ダミーの測定データ更新
                    payload.SequenceNumber += 1;
                    //Console.WriteLine($"SequenceNumber:{payload.SequenceNumber}");
                    payload.Temperature.Raw = random.Next(2500, 3000);
                    payload.RelativeHumidity.Raw = random.Next(4000, 5000);
                    payload.AmbientLight.Raw = random.Next(40, 50);
                    payload.BarometricPressure.Raw = random.Next(1000_000, 1013_000);
                    payload.SoundNoise.Raw = random.Next(50_00, 60_00);
                    // ダミーの受信データ
                    var frame = new Frame(payload);// フレームオブジェクトに変換
                    var size = frame.GetSize();
                    var buffer = frame.ToBytes();
                    // リモートオブジェクトに設定
                    try
                    {
                        server.Mutex.WaitOne();
                        server.RemoteObject.SetReceivedData(DateTime.Now, buffer, size);
                    }
                    finally
                    {
                        server.Mutex.ReleaseMutex();
                    }
                }
                await Task.Delay(1000);
            }
        }
        /// <summary>
        /// 多重起動の禁止処理
        /// </summary>
        /// <returns>trueなら多重起動していないので処理続行。falseなら多重起動しているので終了させる。</returns>
        static bool Main_ProhibitionOfMultipleActivation()
        {
            ProhibitionOfMultipleActivationMutex = new Mutex(false, ProhibitionOfMultipleActivationMutexName);
            if (ProhibitionOfMultipleActivationMutex.WaitOne(0, false) == false)
            {
                Console.WriteLine("多重起動はできません。");
                Console.WriteLine("何かのキーを押すと終了します。");
                Console.ReadLine();
                return false;
            }
            return true;
        }
        #region SerialPort
        /// <summary>
        /// 定期的にデータを取得
        /// </summary>
        static async void EnvironmentalSensorCommunication(CancellationToken cancelToken)
        {
            while (true)
            {
                // キャンセルリクエストが来たらキャンセル
                if (cancelToken.IsCancellationRequested)
                {
                    break;
                }
                if (serialPort.IsOpen)
                {
                    var payload = new LatestDataLongCommandPayload();
                    var frame = new Frame(payload);
                    var buffer = frame.ToBytes();
                    serialPort.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    Console.WriteLine("閉じてる");
                }
                await Task.Delay(1000);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private static void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            DebugWriteLine("ErrorReceived");
        }
        /// <summary>
        /// 
        /// </summary>
        private static void SerialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            DebugWriteLine("PinChanged");
        }
        /// <summary>
        /// データが受信された
        /// </summary>
        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DebugWriteLine("DataReceived");
            var now = DateTime.Now;
            try
            {
                var logFilePath = $"Logs/{now.ToString("yyyyMMdd")}.csv";
                if (logFilePath != csvLogger.FilePath)
                {
                    // 日付が変わったらファイル名変更
                    csvLogger = new CsvLogger(logFilePath);
                    LogHeader();
                }

                // 受信データの蓄積
                var receivedBuffer = serialPortReceivedBuffer;

                using (var memoryStream = new MemoryStream())
                {
                    byte[] readBuffer = new byte[serialPort.ReadBufferSize];
                    int readSize = 0;
                    DebugWriteLine($"{nameof(serialPort)}.{nameof(serialPort.BytesToRead)}={serialPort.BytesToRead}");
                    while (serialPort.BytesToRead > 0)
                    {
                        readSize = serialPort.Read(readBuffer, 0, readBuffer.Length);
                        memoryStream.Write(readBuffer, 0, readSize);
                    }
                    // 蓄積
                    receivedBuffer.AddRange(memoryStream.ToArray());
                }
                // 蓄積したデータが必要最小サイズを超えたら、フレームオブジェクトに変換しログ出力
                DebugWriteLine($"{nameof(receivedBuffer)}.{nameof(receivedBuffer.Count)}={receivedBuffer.Count}");
                while (receivedBuffer.Count > Frame.MinimumSize)
                {
                    try
                    {
                        var buffer = receivedBuffer.ToArray();
                        // フレームオブジェクトに変換
                        var frame = new Frame(buffer);
                        var size = frame.GetSize();
                        // リモートオブジェクトに設定
                        try
                        {
                            server.Mutex.WaitOne();
                            server.RemoteObject.SetReceivedData(now, buffer, size);
                        }
                        finally
                        {
                            server.Mutex.ReleaseMutex();
                        }
                        // ログに出力
                        LogFrame(now, frame);
                        // 読み込み済みデータを削除
                        if (size > 0)
                        {
                            receivedBuffer.RemoveRange(0, size);
                        }
                        else
                        {
                            // 中途半端なデータの場合ループを抜ける＝次のデータ受信で続きをする。
                            break;
                        }
                    }
                    catch (NotSupportedException)
                    {
                        // 無視
                    }
                    catch (DamagedDataException)
                    {
                        // 中途半端なデータの場合ループを抜ける＝次のデータ受信で続きをする。
                        break;
                    }
                    catch (Exception ex)
                    {
                        ConsoleWrite(now, ex);
                    }
                    // 次のヘッダーまでを削除
                    RemoveToNextHeader(receivedBuffer);
                }
                DebugWriteLine($"{nameof(receivedBuffer)}.{nameof(receivedBuffer.Count)}={receivedBuffer.Count}");
            }
            catch (Exception ex)
            {
                ConsoleWrite(now, ex);
            }
        }
        /// <summary>
        /// フレームの情報をログに出力
        /// </summary>
        /// <param name="now">受信日時</param>
        /// <param name="frame">受信したフレーム</param>
        static void LogFrame(DateTime now, Frame frame)
        {
            if (frame.Payload is ErrorResponsePayload)
            {
                var payload = frame.Payload as ErrorResponsePayload;
                Log(now, payload);
            }
            else if (frame.Payload is LatestDataLongResponsePayload)
            {
                var payload = frame.Payload as LatestDataLongResponsePayload;
                Log(now, payload);
            }
        }
        /// <summary>
        /// 次のヘッダーまでを削除
        /// ヘッダーが無い場合は、すべて削除
        /// </summary>
        /// <param name="buffer"></param>
        static int IndexOfNextHeader(IReadOnlyList<byte> buffer)
        {
            var magicNumber = BitConverter.GetBytes(Frame.MagicNumber);
            for (int i = 0; i < buffer.Count - 1; i++)
            {
                if (buffer[i] == magicNumber[0])
                {
                    if (buffer[i + 1] == magicNumber[1])
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// 次のヘッダーまでを削除
        /// ヘッダーが無い場合は、すべて削除
        /// </summary>
        /// <param name="buffer"></param>
        static void RemoveToNextHeader(List<byte> buffer)
        {
            var count = IndexOfNextHeader(buffer);
            if (count == 0)
            {
                // 今の位置がヘッダーなら何もしない
            }
            else if (count < 0)
            {
                // ヘッダーが無いならクリア
                buffer.Clear();
            }
            else
            {
                buffer.RemoveRange(0, count);
            }
        }
        #endregion SerialPort
        #region Log
        static void LogHeader()
        {
            csvLogger.AppendLine(
                "日時",
                "DamagedData",
                "メッセージ",
                "データの16進数表記"
                );
            csvLogger.AppendLine(
                "日時",
                "Error",
                "Code"
                );
            csvLogger.AppendLine(
                "日時",
                "LatestDataLong",
                "SequenceNumber",
                "気温[℃]",
                "相対湿度[％]",
                "照度[ルクス]",
                "気圧[hPa]",
                "雑音[dB]",
                "総揮発性有機化学物量相当値[ppd]",
                "二酸化炭素換算の数値[ppm]",
                "不快指数",
                "熱中症警戒度[℃]",
                "振動情報",
                "スペクトル強度[kine]",
                "PGA",
                "SeismicIntensity",
                "TemperatureFlag",
                "RelativeHumidityFlag",
                "AmbientLightFlag",
                "BarometricPressureFlag",
                "SoundNoiseFlag",
                "eTVOCFlag",
                "eCO2Flag",
                "DiscomfortIndexFlag",
                "HeatStrokeFlag",
                "SIValueFlag",
                "PGAFlag",
                "SeismicIntensityFlag"
                );
        }
        static void LogDamagedData(DateTime now, string message, byte[] readBuffer, int readSize)
        {
            if (logMode.HasFlag(LogFlags.DamagedData) == false)
            {
                return;
            }
            csvLogger.AppendLine(
                now.ToString(DateTimeFormat),
                "DamagedData",
                message,
                readBuffer.ToJoinedString(" ", "X2")
                );
        }
        static void Log(DateTime now, ErrorResponsePayload payload)
        {
            if (logMode.HasFlag(LogFlags.Error) == false)
            {
                return;
            }
            csvLogger.AppendLine(
                now.ToString(DateTimeFormat),
                "Error",
                payload.Code.ToString()
                );
        }
        static void Log(DateTime now, LatestDataLongResponsePayload payload)
        {
            if (logMode.HasFlag(LogFlags.LatestDataLong) == false)
            {
                return;
            }
            csvLogger.AppendLine(
                $"{now.ToString(DateTimeFormat)}",
                $"LatestDataLong",
                $"{payload.SequenceNumber}",
                $"{payload.Temperature.Value}",
                $"{payload.RelativeHumidity.Value}",
                $"{payload.AmbientLight.Value}",
                $"{payload.BarometricPressure.Value}",
                $"{payload.SoundNoise.Value}",
                $"{payload.eTVOC.Value}",
                $"{payload.eCO2.Value}",
                $"{payload.DiscomfortIndex.Value}",
                $"{payload.HeatStroke.Value}",
                $"{payload.VibrationInformation}",
                $"{payload.SIValue.Value}",
                $"{payload.PGA.Value}",
                $"{payload.SeismicIntensity.Value}",
                $"{payload.TemperatureFlag}",
                $"{payload.RelativeHumidityFlag}",
                $"{payload.AmbientLightFlag}",
                $"{payload.BarometricPressureFlag}",
                $"{payload.SoundNoiseFlag}",
                $"{payload.eTVOCFlag}",
                $"{payload.eCO2Flag}",
                $"{payload.DiscomfortIndexFlag}",
                $"{payload.HeatStrokeFlag}",
                $"{payload.SIValueFlag}",
                $"{payload.PGAFlag}",
                $"{payload.SeismicIntensityFlag}"
                );
        }
        #endregion Log
        #region Debug
        static void ConsoleWrite(DateTime now, Exception ex)
        {
            Console.WriteLine(now.ToString());
            Console.WriteLine(ex.ToString());
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        static void DebugWriteLine(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
#if DEBUG
        /// <summary>
        /// RemoveToNextHeader関数のテスト
        /// </summary>
        static void RemoveToNextHeaderTest()
        {
            var magicNumber = BitConverter.GetBytes(Frame.MagicNumber);
            var random = new Random();
            var buffer = new byte[20];
            random.NextBytes(buffer);
            // ヘッダーが無い場合すべて削除
            var list = new List<byte>(buffer);
            RemoveToNextHeader(list);
            Debug.Assert(list.Count == 0);
            // 
            buffer[10] = magicNumber[0];
            buffer[11] = magicNumber[1];
            list = new List<byte>(buffer);
            RemoveToNextHeader(list);
            Debug.Assert(list.Count == buffer.Length - 10);
            // 先頭の場合、何もしない
            buffer[0] = magicNumber[0];
            buffer[1] = magicNumber[1];
            list = new List<byte>(buffer);
            RemoveToNextHeader(list);
            Debug.Assert(list.Count == buffer.Length);
        }
#endif
        #endregion Debug
    }
}
