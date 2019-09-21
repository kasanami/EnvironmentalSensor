using EnvironmentalSensor;
using EnvironmentalSensor.Ipc;
using EnvironmentalSensor.USB;
using EnvironmentalSensor.USB.Payloads;
using Ksnm.Utilities;
using System;
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
        static byte[] readBuffer = new byte[serialPort.ReadBufferSize];

        static Server server = new Server();
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
            // リース期間を無限に設定(ゼロで無限になる)
            LifetimeServices.LeaseTime = TimeSpan.Zero;

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
        /// データが受信された
        /// </summary>
        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var now = DateTime.Now;
            var logFilePath = $"Logs/{DateTime.Now.ToString("yyyyMMdd")}.csv";
            if (logFilePath != csvLogger.FilePath)
            {
                // 日付が変わったらファイル名変更
                csvLogger = new CsvLogger(logFilePath);
                LogHeader();
            }
            var readSize = serialPort.Read(readBuffer, 0, readBuffer.Length);

            DebugWriteLine("DataReceived");
            DebugWriteLine($"readSize={readSize}");

            if (readSize > 0)
            {
                try
                {
                    var frame = new Frame(readBuffer, 0, readSize);
                    // リモートオブジェクトに設定
                    server.RemoteObject.Set(frame.Payload);
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
                }
                catch (NotSupportedException)
                {
                    // 無視
                }
                catch (DamagedDataException ex)
                {
                    LogDamagedData(now, ex.Message, readBuffer, readSize);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
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
                $"{payload.Temperature * payload.TemperatureUnit}",
                $"{payload.RelativeHumidity * payload.RelativeHumidityUnit}",
                $"{payload.AmbientLight}",
                $"{payload.BarometricPressure * payload.BarometricPressureUnit}",
                $"{payload.SoundNoise * payload.SoundNoiseUnit}",
                $"{payload.eTVOC}",
                $"{payload.eCO2}",
                $"{payload.DiscomfortIndex * payload.DiscomfortIndexUnit}",
                $"{payload.HeatStroke * payload.HeatStrokeUnit}",
                $"{payload.VibrationInformation}",
                $"{payload.SIValue * payload.SIValueUnit}",
                $"{payload.PGA * payload.PGAUnit}",
                $"{payload.SeismicIntensity * payload.SeismicIntensityUnit}",
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

        static void DebugWriteLine(string message)
        {
#if DEBUG
            //Console.WriteLine(message);
#endif
        }
    }
}
