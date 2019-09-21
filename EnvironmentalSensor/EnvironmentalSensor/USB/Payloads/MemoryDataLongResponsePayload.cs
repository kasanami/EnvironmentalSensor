using System;
using System.IO;
using System.Text;
using Ksnm.ExtensionMethods.System.IO.Stream;

namespace EnvironmentalSensor.USB.Payloads
{
    public class MemoryDataLongResponsePayload : DataLongResponsePayload
    {
        #region Dataの内容
        /// <summary>
        /// Range: 0x00000001 to 0x7FFFFFFF
        /// If data error, MSB is 1
        /// </summary>
        public UInt32 MemoryIndex { get; private set; }
        /// <summary>
        /// Range: 0x1 to 0xFFFFFFFFFFFFFFFF
        /// </summary>
        public UInt64 TimeCounter { get; private set; }
        /// <summary>
        /// エラー
        /// </summary>
        public bool IsError { get => (MemoryIndex & 0x800_0000) != 0; }
        #endregion Dataの内容
        /// <summary>
        /// 指定されたストリームから初期化
        /// </summary>
        /// <param name="binaryReader">Payloadが含まれるストリーム</param>
        /// <param name="offset">Payloadの先頭位置のオフセット</param>
        public MemoryDataLongResponsePayload(BinaryReader binaryReader, int offset) : base(binaryReader, offset)
        {
            // Command+Addressのサイズ移動する
            binaryReader.BaseStream.Seek(offset + 3, SeekOrigin.Begin);
            MemoryIndex = binaryReader.ReadUInt32();
            TimeCounter = binaryReader.ReadUInt16();
        }

        public override string ToString()
        {
            var text = new StringBuilder();
            text.AppendLine("{");
            text.AppendLine(nameof(MemoryIndex) + "=" + MemoryIndex);
            text.AppendLine(nameof(TimeCounter) + "=" + TimeCounter);
            text.AppendLine(nameof(IsError) + "=" + IsError);
            text.AppendLine(ToInnerString());
            text.AppendLine("}");
            return text.ToString();
        }
    }
}
