using System;
using System.Collections.Generic;

namespace EnvironmentalSensor.USB.Payloads
{
    /// <summary>
    /// FLASH memory に保存されているセンシングデータを取得する
    /// マニュアル:4.4.1 Memory data long (Address: 0x500E)
    /// </summary>
    public class MemoryDataLongCommandPayload : CommandPayload
    {
        public override FrameCommand Command { get => FrameCommand.Read; }
        public override FrameAddress Address { get => FrameAddress.MemoryDataLong; }
        public override byte[] Data
        {
            get
            {
                List<byte> data = new List<byte>();
                data.AddRange(BitConverter.GetBytes(StartMemoryIndex));
                data.AddRange(BitConverter.GetBytes(EndMemoryIndex));
                return data.ToArray();
            }
        }
        #region Dataの内容
        /// <summary>
        /// 範囲: 0x00000001 to 0xFFFFFFFF
        /// Last index <= Start index <= End index
        /// </summary>
        public UInt32 StartMemoryIndex { get; set; }
        /// <summary>
        /// 範囲: 0x00000001 to 0xFFFFFFFF
        /// Start index <= End index <= Latest index
        /// </summary>
        public UInt32 EndMemoryIndex { get; set; }
        #endregion Dataの内容
    }
}
