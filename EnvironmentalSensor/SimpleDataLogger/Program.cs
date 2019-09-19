﻿using EnvironmentalSensor;
using EnvironmentalSensor.USB;
using EnvironmentalSensor.USB.Payloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDataLogger
{
    class Program
    {
        const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";

        static SerialPort serialPort = new SerialPort();
        static byte[] readBuffer = new byte[serialPort.ReadBufferSize];
        static StreamWriter logStream;

        [Flags]
        enum LogMode : uint
        {
            Error = 0x0000_0001,
            LatestDataLong = 0x0000_0002,
            DamagedData = 0x8000_0000,
            All = 0xFFFF_FFFF,
        }
        static LogMode logMode = LogMode.All;

        static void Main(string[] args)
        {
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
            serialPort.DataReceived += SerialPort_DataReceived;
            /// マニュアル:4.1. Communication specification
            serialPort.BaudRate = 115200;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.Handshake = Handshake.None;// フロー制御：None
            serialPort.PortName = portName;
            serialPort.Open();
            // 
            using (logStream = new StreamWriter("log.txt", true, Encoding.UTF8))
            {
                LogHeader();
                EnvironmentalSensorCommunication();
                Console.WriteLine("終了するにはなにかキーを入力");
                Console.ReadLine();
            }
            // 終了
            serialPort.Close();
        }

        static async void EnvironmentalSensorCommunication()
        {
            while (true)
            {
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
            var readSize = serialPort.Read(readBuffer, 0, readBuffer.Length);
#if DEBUG
            Console.WriteLine("DataReceived");
            Console.WriteLine($"readSize={readSize}");
#endif
            if (readSize > 0)
            {
                try
                {
                    var frame = new Frame(readBuffer, 0, readSize);

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
            logStream.WriteLine(
                $"日時," +
                $"DamagedData," +
                $"メッセージ," +
                $"データの16進数表記,"
                );
            logStream.WriteLine(
                $"日時," +
                $"Error," +
                $"Code,"
                );
            logStream.WriteLine(
                $"日時," +
                $"LatestDataLong," +
                $"SequenceNumber," +
                $"気温[℃]," +
                $"相対湿度[％]," +
                $"照度[ルクス]," +
                $"気圧[hPa]," +
                $"雑音[dB]," +
                $"総揮発性有機化学物量相当値[ppd]," +
                $"二酸化炭素換算の数値[ppm]," +
                $"不快指数," +
                $"熱中症警戒度[℃]," +
                $"振動情報," +
                $"スペクトル強度[kine]," +
                $"PGA," +
                $"SeismicIntensity," +
                $"TemperatureFlag," +
                $"RelativeHumidityFlag," +
                $"AmbientLightFlag," +
                $"BarometricPressureFlag," +
                $"SoundNoiseFlag," +
                $"eTVOCFlag," +
                $"eCO2Flag," +
                $"DiscomfortIndexFlag," +
                $"HeatStrokeFlag," +
                $"SIValueFlag," +
                $"PGAFlag," +
                $"SeismicIntensityFlag,"
                );
        }
        static void LogDamagedData(DateTime now, string message, byte[] readBuffer, int readSize)
        {
            if (logMode.HasFlag(LogMode.DamagedData) == false)
            {
                return;
            }
            var readBufferText = new StringBuilder();
            for (int i = 0; i < readSize; i++)
            {
                readBufferText.Append(readBuffer[i].ToString("X2") + " ");
            }
            logStream.WriteLine(
                $"{now.ToString(DateTimeFormat)}," +
                $"DamagedData," +
                $"{message}," +
                $"{readBufferText},"
                );
        }
        static void Log(DateTime now, ErrorResponsePayload payload)
        {
            if (logMode.HasFlag(LogMode.Error) == false)
            {
                return;
            }
            logStream.WriteLine(
                $"{now.ToString(DateTimeFormat)}," +
                $"Error," +
                $"{payload.Code},"
                );
        }
        static void Log(DateTime now, LatestDataLongResponsePayload payload)
        {
            if (logMode.HasFlag(LogMode.LatestDataLong) == false)
            {
                return;
            }
            logStream.WriteLine(
                $"{now.ToString(DateTimeFormat)}," +
                $"LatestDataLong," +
                $"{payload.SequenceNumber}," +
                $"{payload.Temperature * 0.01}," +
                $"{payload.RelativeHumidity * 0.01}," +
                $"{payload.AmbientLight}," +
                $"{payload.BarometricPressure * 0.001}," +
                $"{payload.SoundNoise * 0.01}," +
                $"{payload.eTVOC}," +
                $"{payload.eCO2}," +
                $"{payload.DiscomfortIndex * 0.01}," +
                $"{payload.HeatStroke * 0.01}," +
                $"{payload.VibrationInformation}," +
                $"{payload.SIValue * 0.1}," +
                $"{payload.PGA * 0.1}," +
                $"{payload.SeismicIntensity * 0.001}," +
                $"{payload.TemperatureFlag}," +
                $"{payload.RelativeHumidityFlag}," +
                $"{payload.AmbientLightFlag}," +
                $"{payload.BarometricPressureFlag}," +
                $"{payload.SoundNoiseFlag}," +
                $"{payload.eTVOCFlag}," +
                $"{payload.eCO2Flag}," +
                $"{payload.DiscomfortIndexFlag}," +
                $"{payload.HeatStrokeFlag}," +
                $"{payload.SIValueFlag}," +
                $"{payload.PGAFlag}," +
                $"{payload.SeismicIntensityFlag},"
                );
        }
    }
}