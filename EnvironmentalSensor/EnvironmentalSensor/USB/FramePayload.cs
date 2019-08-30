using System;
using System.Collections.Generic;

namespace EnvironmentalSensor.USB
{
    /// <summary>
    /// 付加的情報を除いた、データ本体
    /// </summary>
    public abstract class FramePayload
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
        /// </summary>
        public virtual byte[] Data { get; set; }
        /// <summary>
        /// このインスタンス情報をバイト配列に変換
        /// </summary>
        /// <returns>バイト配列</returns>
        public byte[] ToBytes()
        {
            var data = new List<byte>();
            data.Add((byte)Command);
            data.AddRange(BitConverter.GetBytes((ushort)Address));
            data.AddRange(Data);
            return data.ToArray();
        }
    }
}
