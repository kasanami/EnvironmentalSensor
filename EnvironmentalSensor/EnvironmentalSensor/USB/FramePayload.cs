using Ksnm.ExtensionMethods.System.Collections.Generic.Enumerable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EnvironmentalSensor.USB
{
    /// <summary>
    /// 付加的情報を除いた、データ本体
    /// </summary>
    public abstract class FramePayload : MarshalByRefObject, IEquatable<FramePayload>
    {
        /// <summary>
        /// Read, Write を指定する
        /// </summary>
        public virtual FrameCommand Command { get; set; }
        /// <summary>
        /// 実行する内容に応じて Address を指定する
        /// </summary>
        public virtual FrameAddress Address { get; set; }
        /// <summary>
        /// Address により内容が異なる
        /// <para>このプロパティは、継承先クラスでオーバーライドすることを想定している。</para>
        /// <para>継承先クラスでは、このプロパティを参照する際は、循環参照に注意すること。</para>
        /// </summary>
        public virtual byte[] Data { get; set; } = new byte[0];
        /// <summary>
        /// このインスタンス情報をバイト配列に変換
        /// </summary>
        /// <returns>バイト配列</returns>
        public byte[] ToBytes()
        {
            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write((byte)Command);
                binaryWriter.Write((ushort)Address);
                binaryWriter.Write(Data);
                return memoryStream.ToArray();
            }
        }

        #region IEquatable
        public virtual bool Equals(FramePayload other)
        {
            if (Command != other.Command) { return false; }
            if (Address != other.Address) { return false; }
            if (Data.Equals(other.Data) == false) { return false; }
            return true;
        }
        #endregion IEquatable

        #region object
        /// <summary>
        /// 指定したオブジェクトが、現在のオブジェクトと等しいかどうかを判断します。
        /// </summary>
        /// <returns>指定したオブジェクトが現在のオブジェクトと等しい場合は true。それ以外の場合は false。</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is FramePayload)
            {
                return Equals((FramePayload)obj);
            }
            return false;
        }
        /// <summary>
        /// このインスタンスのハッシュ コードを返します。
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = 0;
            hashCode ^= (int)Command;
            hashCode ^= (int)Address;
            hashCode ^= Data.GetHashCode();
            return hashCode;
        }
        /// <summary>
        /// 文字列に変換
        /// </summary>
        public override string ToString()
        {
            var text = new StringBuilder();
            text.AppendLine("{");
            text.AppendLine($"{nameof(Command)}={Command},");
            text.AppendLine($"{nameof(Address)}={Address},");
            text.AppendLine($"{nameof(Data)}={Data?.ToDebugString() ?? "null"},");
            text.AppendLine("}");
            return text.ToString();
        }
        #endregion object
    }
}
