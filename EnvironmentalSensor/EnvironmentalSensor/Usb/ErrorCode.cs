namespace EnvironmentalSensor.Usb
{
    /// <summary>
    /// エラーレスポンス
    /// マニュアル：Table 77 Error code
    /// </summary>
    public enum ErrorCode : byte
    {
        /// <summary>
        /// CRC-16 calculation が誤っている場合
        /// </summary>
        CRC = 0x01,
        /// <summary>
        ///  Command に Read, Write 以外が指定された場合
        ///  この場合 Command は Unknown となる
        /// </summary>
        Command = 0x02,
        /// <summary>
        /// Address list に記載されていない Address が指定された場合
        /// </summary>
        Address = 0x03,
        /// <summary>
        /// Address で指定されている Length が誤っている場合
        /// </summary>
        Length = 0x04,
        /// <summary>
        /// Write の書き込み範囲外を指定された場合
        /// </summary>
        Data = 0x05,
        /// <summary>
        /// FLASH Memory アクセス中など内部処理している場合
        /// </summary>
        Busy = 0x06,
    }
}
