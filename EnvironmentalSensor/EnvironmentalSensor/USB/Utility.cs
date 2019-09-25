using System.IO.Ports;

namespace EnvironmentalSensor.USB
{
    public class Utility
    {
        /// <summary>
        /// SerialPortを設定する
        /// マニュアル:4.1. Communication specification
        /// </summary>
        /// <param name="serialPort"></param>
        public static void SettingSerialPort(SerialPort serialPort)
        {
            serialPort.BaudRate = 115200;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.Handshake = Handshake.None;// フロー制御：None
        }
    }
}
