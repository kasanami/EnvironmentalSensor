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
        /// <param name="index">buffer内のPayloadの位置</param>
        protected ResponsePayload(byte[] buffer, int index)
        {
            Command = (FrameCommand)buffer[index];
            Address = (FrameAddress)BitConverter.ToUInt16(buffer, index + 1);
            Data = new byte[buffer.Length - 3];
            Buffer.BlockCopy(buffer, index + 3, Data, 0, Data.Length);
        }
        /// <summary>
        /// 指定したバッファから、インスタンスを生成する。
        /// </summary>
        /// <param name="buffer">Payloadが含まれるバッファ</param>
        /// <param name="index">buffer内のPayloadの位置</param>
        /// <returns></returns>
        public static ResponsePayload Create(byte[] buffer, int index)
        {
            var command = (FrameCommand)buffer[index];
            var address = (FrameAddress)BitConverter.ToUInt16(buffer, index + 1);
            if (command.HasFlag(FrameCommand.Error))
            {
                return new ErrorResponsePayload(buffer, index);
            }
            else if (address == FrameAddress.MemoryDataLong)
            {
                return new MemoryDataLongResponsePayload(buffer, index);
            }
            else if (address == FrameAddress.MemoryDataShort)
            {
                throw new NotImplementedException($"address={address}");
            }
            else if (address == FrameAddress.LatestDataLong)
            {
                return new LatestDataLongResponsePayload(buffer, index);
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
                return new ResponsePayload(buffer, 0);
            }
        }
    }
}
