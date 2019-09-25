﻿using EnvironmentalSensor;
using EnvironmentalSensor.Ipc;
using EnvironmentalSensor.USB;
using EnvironmentalSensor.USB.Payloads;
using Ksnm.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpcServer
{
    /// <summary>
    /// サーバープログラム
    /// </summary>
    class Program
    {
        const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";

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
        static LogFlags logMode = LogFlags.All;
        #endregion ログ関係

        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine($"IpcServer DEBUG mode");
            RemoveToNextHeaderTest();
#else
            Console.WriteLine($"IpcServer");
#endif
#if false
            Console.WriteLine($"{nameof(LifetimeServices.LeaseManagerPollTime)}={LifetimeServices.LeaseManagerPollTime}");
            Console.WriteLine($"{nameof(LifetimeServices.LeaseTime)}={LifetimeServices.LeaseTime}");
            Console.WriteLine($"{nameof(LifetimeServices.RenewOnCallTime)}={LifetimeServices.RenewOnCallTime}");
            Console.WriteLine($"{nameof(LifetimeServices.SponsorshipTimeout)}={LifetimeServices.SponsorshipTimeout}");
#endif
            // リース期間を無限に設定(ゼロで無限になる)
            LifetimeServices.LeaseTime = TimeSpan.Zero;
            server = new Server();

            var portName = "";
            while (true)
            {
                Console.WriteLine($"ポート名を入力[COM1など]");
                portName = Console.ReadLine();
                Console.WriteLine($"{portName} でよろしいですか？[Y/N]");
                var read = Console.ReadLine();
                if (read.ToUpper() == "Y")
                {
                    break;
                }
            }
            // シリアルポートを設定して通信を開始
            {
                serialPort.ErrorReceived += SerialPort_ErrorReceived;
                serialPort.PinChanged += SerialPort_PinChanged;
                serialPort.DataReceived += SerialPort_DataReceived;
                /// マニュアル:4.1. Communication specification
                serialPort.BaudRate = 115200;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;
                serialPort.Handshake = Handshake.None;// フロー制御：None
                serialPort.PortName = portName;
                serialPort.Open();
            }
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
                    Console.WriteLine("停止するにはなにかキーを入力");
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
            // 終了
            serialPort.Close();
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

            var receivedBuffer = serialPortReceivedBuffer;

            var now = DateTime.Now;
            var logFilePath = $"Logs/{DateTime.Now.ToString("yyyyMMdd")}.csv";
            if (logFilePath != csvLogger.FilePath)
            {
                // 日付が変わったらファイル名変更
                csvLogger = new CsvLogger(logFilePath);
                LogHeader();
            }

            byte[] readBuffer = new byte[serialPort.ReadBufferSize];

            using (var memoryStream = new MemoryStream())
            {
                int readSize = 0;
                DebugWriteLine($"{nameof(serialPort)}.{nameof(serialPort.BytesToRead)}={serialPort.BytesToRead}");
                while (serialPort.BytesToRead > 0)
                {
                    try
                    {
                        readSize = serialPort.Read(readBuffer, 0, readBuffer.Length);
                        memoryStream.Write(readBuffer, 0, readSize);
                    }
                    catch (Exception ex)
                    {
                        DebugWriteLine($"{ex.Message}\n{ex.ToString()}");
                        return;
                    }
                }
                // 蓄積
                receivedBuffer.AddRange(memoryStream.ToArray());
            }

            DebugWriteLine($"{nameof(receivedBuffer)}.{nameof(receivedBuffer.Count)}={receivedBuffer.Count}");

            if (receivedBuffer.Count > Frame.MinimumSize)
            {
                var buffer = receivedBuffer.ToArray();
                try
                {
                    var frame = new Frame(buffer, 0, buffer.Length);
                    // リモートオブジェクトに設定
                    try
                    {
                        server.Mutex.WaitOne();
                        server.RemoteObject.Set(frame.Payload);
                    }
                    finally
                    {
                        server.Mutex.ReleaseMutex();
                    }
                    // ログに出力
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
                    // 読み込み済みデータを削除
                    receivedBuffer.RemoveRange(0, frame.Length + 4);
                }
                catch (NotSupportedException)
                {
                    // 無視
                }
                catch (DamagedDataException ex)
                {
                    LogDamagedData(now, ex.Message, buffer, buffer.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                // 次のヘッダーまでを削除
                RemoveToNextHeader(receivedBuffer);
            }
            DebugWriteLine($"{nameof(receivedBuffer)}.{nameof(receivedBuffer.Count)}={receivedBuffer.Count}");
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
            var readBufferText = new StringBuilder();
            for (int i = 0; i < readSize; i++)
            {
                readBufferText.Append(readBuffer[i].ToString("X2") + " ");
            }
            csvLogger.AppendLine(
                now.ToString(DateTimeFormat),
                "DamagedData",
                message,
                readBufferText.ToString()
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
