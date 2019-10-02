using System;
using System.IO;

namespace EnvironmentalSensor.Usb.Payloads
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
        /// 指定のストリームから初期化
        /// </summary>
        /// <param name="binaryReader">Payloadが含まれるストリーム</param>
        /// <param name="offset">Payloadの先頭位置のオフセット</param>
        protected ResponsePayload(BinaryReader binaryReader, int offset)
        {
            binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
            Command = (FrameCommand)binaryReader.ReadByte();
            Address = (FrameAddress)binaryReader.ReadUInt16();
            Data = binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));
        }
        /// <summary>
        /// 指定したストリームから、インスタンスを生成する。
        /// </summary>
        /// <param name="binaryReader">Payloadが含まれるストリーム</param>
        /// <param name="offset">Payloadの先頭位置のオフセット</param>
        public static ResponsePayload Create(BinaryReader binaryReader, int offset)
        {
            binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
            var command = (FrameCommand)binaryReader.ReadByte();
            var address = (FrameAddress)binaryReader.ReadUInt16();
            // 各ペイロードのインスタンスを生成
            if (command.HasFlag(FrameCommand.Error))
            {
                return new ErrorResponsePayload(binaryReader, offset);
            }
            else if (address == FrameAddress.MemoryDataLong)
            {
                return new MemoryDataLongResponsePayload(binaryReader, offset);
            }
            else if (address == FrameAddress.MemoryDataShort)
            {
                throw new NotImplementedException($"address={address}");
            }
            else if (address == FrameAddress.LatestDataLong)
            {
                return new LatestDataLongResponsePayload(binaryReader, offset);
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
                return new ResponsePayload(binaryReader, offset);
            }
        }
    }
}
