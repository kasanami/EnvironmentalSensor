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
        /// Sequence number は Memory index と非同期の管理番号であり測定毎(1sec 毎)にインクリメントされる．
        /// 電源投入後から USB 型環境センサ内部で自動的に更新され 0x00 からスタートする．
        /// 0xFF まで達すると次は 0x00 となり以降この動作を繰り返す．
        /// Range: 0x00 to 0xFF
        /// マニュアル：Table 12 Memory index and Sequence number
        /// </summary>
        public UInt8 SequenceNumber;
        #endregion Dataの内容
        /// <summary>
        /// 
        /// </summary>
        public LatestDataLongResponsePayload(byte sequenceNumber)
        {
            SequenceNumber = sequenceNumber;

            Command = FrameCommand.Read;
            Address = FrameAddress.LatestDataLong;

            UpdateData();
        }
        /// <summary>
        /// Dataメンバーを更新
        /// </summary>
        public override void UpdateData()
        {
            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(SequenceNumber);
                WriteOn(binaryWriter);
                Data = memoryStream.ToArray();
            }
        }
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
