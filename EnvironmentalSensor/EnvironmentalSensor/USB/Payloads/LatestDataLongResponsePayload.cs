using System.IO;
using System.Text;
using Ksnm.ExtensionMethods.System.IO.Stream;
using UInt8 = System.Byte;

namespace EnvironmentalSensor.USB.Payloads
{
    /// <summary>
    /// 最新データの返答
    /// </summary>
    public class LatestDataLongResponsePayload : DataLongResponsePayload
    {
        #region Dataの内容
        /// <summary>
        /// ？
        /// Range: 0x00 to 0xFF
        /// </summary>
        public UInt8 SequenceNumber;
        #endregion Dataの内容
        /// <summary>
        /// 指定されたストリームから初期化
        /// </summary>
        /// <param name="binaryReader">Payloadが含まれるストリーム</param>
        /// <param name="offset">Payloadの先頭位置のオフセット</param>
        public LatestDataLongResponsePayload(BinaryReader binaryReader, int offset) : base(binaryReader, offset)
        {
            SequenceNumber = Data[0];
        }

        public override string ToString()
        {
            var text = new StringBuilder();
            text.AppendLine("{");
            text.AppendLine(nameof(SequenceNumber) + "=" + SequenceNumber.ToString());
            text.AppendLine(ToInnerString());
            text.AppendLine("}");
            return text.ToString();
        }
    }
}
