using System.IO;
using System.Text;

namespace EnvironmentalSensor.Usb.Payloads
{
    /// <summary>
    /// エラー返答
    /// マニュアル:4.3.5 Payload frame format [Error Response from 2JCIE-BU01] 
    /// </summary>
    public class ErrorResponsePayload : ResponsePayload
    {
        /// <summary>
        /// Error 内容
        /// </summary>
        public ErrorCode Code { get; private set; }
        /// <summary>
        /// 指定のストリームから初期化
        /// </summary>
        /// <param name="binaryReader">Payloadが含まれるストリーム</param>
        /// <param name="offset">Payloadの先頭位置のオフセット</param>
        public ErrorResponsePayload(BinaryReader binaryReader, int offset) : base(binaryReader, offset)
        {
            // Command+Addressのサイズ移動する
            binaryReader.BaseStream.Seek(offset + 3, SeekOrigin.Begin);
            Code = (ErrorCode)binaryReader.ReadByte();
        }
        public override string ToString()
        {
            var text = new StringBuilder();
            text.AppendLine("{");
            text.AppendLine($"{nameof(Command)}={Command}");
            text.AppendLine($"{nameof(Address)}={Address}");
            text.AppendLine($"{nameof(Code)}={Code}");
            text.AppendLine("}");
            return text.ToString();
        }
    }
}
