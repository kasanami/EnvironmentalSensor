namespace EnvironmentalSensor.USB
{
    /// <summary>
    /// 実行する内容
    /// サイズ=2バイト
    /// </summary>
    public enum FrameAddress : ushort
    {
        /// <summary>
        /// FLASH memory に保存されているセンシングデータを取得する．
        /// </summary>
        MemoryDataLong = 0x500E,
        MemoryDataShort = 0x500F,
        LatestDataLong = 0x5021,
        LatestDataShort = 0x5022,
        AccelerationMemoryDataHeader = 0x503E,
        AccelerationMemoryData = 0x503F,
        /// <summary>
        /// FLASH memory に保存されているセンシングデータの index 数を取得する．
        /// </summary>
        LatestMemoryInformation = 0x5004,
    }
}
