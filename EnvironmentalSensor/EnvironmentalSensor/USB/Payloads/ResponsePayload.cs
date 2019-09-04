using System;

namespace EnvironmentalSensor.USB.Payloads
{
    /// <summary>
    /// 返答のペイロード
    /// </summary>
    public class ResponsePayload : FramePayload
    {
        protected ResponsePayload()
        {
        }
        /// <summary>
        /// 指定のバッファから初期化
        /// </summary>
        /// <param name="buffer">Payloadが含まれるバッファ</param>
        protected ResponsePayload(byte[] buffer)
        {
            Command = (FrameCommand)buffer[0];
            Address = (FrameAddress)BitConverter.ToUInt16(buffer, 1);
            Data = new byte[buffer.Length - 3];
            Buffer.BlockCopy(buffer, 3, Data, 0, Data.Length);
        }
        /// <summary>
        /// 指定したバッファから、インスタンスを生成する。
        /// </summary>
        /// <param name="buffer">Payloadが含まれるバッファ</param>
        public static ResponsePayload Create(byte[] buffer)
        {
            var command = (FrameCommand)buffer[0];
            var address = (FrameAddress)BitConverter.ToUInt16(buffer, 1);
            if (command.HasFlag(FrameCommand.Error))
            {
                return new ErrorResponsePayload(buffer);
            }
            else if (address == FrameAddress.MemoryDataLong)
            {
                return new MemoryDataLongResponsePayload(buffer);
            }
            else if (address == FrameAddress.MemoryDataShort)
            {
                throw new NotImplementedException($"address={address}");
            }
            else if (address == FrameAddress.LatestDataLong)
            {
                return new LatestDataLongResponsePayload(buffer);
            }
            else if (address == FrameAddress.LatestDataShort)
            {
                throw new NotImplementedException($"address={address}");
            }
            else if (address == FrameAddress.AccelerationMemoryDataHeader)
            {
                throw new NotImplementedException($"address={address}");
            }
            else if (address == FrameAddress.AccelerationMemoryData)
            {
                throw new NotImplementedException($"address={address}");
            }
            else
            {
                return new ResponsePayload(buffer);
            }
        }
    }
}
