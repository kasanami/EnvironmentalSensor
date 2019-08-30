using System;
using System.Collections.Generic;

namespace EnvironmentalSensor.USB.Payloads
{
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
        public UInt32 StartMemoryIndex { get; set; }
        public UInt32 EndMemoryIndex { get; set; }
        #endregion Dataの内容
    }
}
