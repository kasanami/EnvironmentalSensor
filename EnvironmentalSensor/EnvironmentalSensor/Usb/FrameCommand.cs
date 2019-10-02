using System;

namespace EnvironmentalSensor.Usb
{
    /// <summary>
    /// Read, Write を指定する
    /// サイズ=1バイト
    /// </summary>
    [Flags]
    public enum FrameCommand : byte
    {
        Read = 0x01,
        Write = 0x02,
        /// <summary>
        /// Error 返答時は Command の MSB を 1 にした値（0x80 を加えた値）となる．
        /// </summary>
        Error = 0x80,
        ReadError = 0x01 | Error,
        WriteError = 0x02 | Error,
        /// <summary>
        /// Read, Write 以外の Command を受信した場合は Unknown(0xFF)で返答する．
        /// </summary>
        Unknown = 0xFF,
    }
}
