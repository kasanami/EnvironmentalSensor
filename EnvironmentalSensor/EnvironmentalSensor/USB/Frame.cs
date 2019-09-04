using EnvironmentalSensor.USB.Payloads;
using Ksnm.Algorithm;
using Ksnm.ExtensionMethods.System.IO.Stream;
using System;
using System.Collections.Generic;
using System.IO;

namespace EnvironmentalSensor.USB
{
    /// <summary>
    /// USB 通信の送受信で使用する共通フレーム
    /// マニュアル:4.3. Frame format
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// Headerに設定する値
        /// </summary>
        const ushort MagicNumber = 0x4252;
        /// <summary>
        /// ASCII コードの”BR”(0x4252)で固定とする
        /// </summary>
        public ushort Header { get; } = MagicNumber;
        /// <summary>
        /// Payload から CRC までのデータ長を指定する
        /// </summary>
        public ushort Length { get; }
        /// <summary>
        /// 付加的情報を除いた、データ本体
        /// </summary>
        public FramePayload Payload { get; }
        /// <summary>
        /// Header から Payload 末尾までの CRC 結果を設定する
        /// </summary>
        public ushort CRC16 { get; }
        /// <summary>
        /// CRC16の計算器
        /// </summary>
        static CRC16 crc16 = new CRC16(CRC16Polynomial.IBM_Reversed, 0xFFFF, 0);
        /// <summary>
        /// 指定のバッファから初期化
        /// </summary>
        /// <param name="buffer">フレームのバッファ</param>
        /// <param name="index">フレームの開始位置</param>
        /// <param name="count">フレームのバイト数</param>
        public Frame(byte[] buffer, int index, int count)
        {
            const int TopSize = 4;
            const int CRC16Size = 2;
            using (var memoryStream = new MemoryStream(buffer, index, count, false))
            {
                Header = memoryStream.ReadUInt16();
                if (Header != MagicNumber)
                {
                    // 対応してるデータではない
                    throw new NotSupportedException($"Header={Header.ToString("X4")}");
                }
                Length = memoryStream.ReadUInt16();
                // CRCチェック
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);// 最初に戻る
                    var tempBuffer = new byte[Length + TopSize - CRC16Size];
                    memoryStream.Read(tempBuffer, 0, tempBuffer.Length);
                    var computedCRC16 = crc16.ComputeHash(tempBuffer);
                    CRC16 = memoryStream.ReadUInt16();
                    if (CRC16 != computedCRC16)
                    {
                        // CRCの結果 データ破損
                        throw new DamagedDataException($"{nameof(CRC16)}={CRC16.ToString("X4")} {nameof(computedCRC16)}={computedCRC16.ToString("X4")}");
                    }
                }
                // Payload読み込み
                {
                    memoryStream.Seek(TopSize, SeekOrigin.Begin);// Payloadの位置に移動
                    var tempBuffer = new byte[Length - CRC16Size];
                    memoryStream.Read(tempBuffer, 0, tempBuffer.Length);
                    Payload = ResponsePayload.Create(tempBuffer);
                }
            }
        }
        /// <summary>
        /// 指定したペイロードから初期化する
        /// </summary>
        /// <param name="payload"></param>
        public Frame(FramePayload payload)
        {
            Payload = payload;
            var payloadBytes = Payload.ToBytes();
            // Payload + CRC16
            Length = (ushort)(payloadBytes.Length + sizeof(ushort));
            // CRC16計算
            var data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(Header));
            data.AddRange(BitConverter.GetBytes(Length));
            data.AddRange(Payload.ToBytes());
            CRC16 = crc16.ComputeHash(data.ToArray());
        }
        /// <summary>
        /// バイト配列に変換する。
        /// </summary>
        /// <returns>バイト配列</returns>
        public byte[] ToBytes()
        {
            var data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(Header));
            data.AddRange(BitConverter.GetBytes(Length));
            data.AddRange(Payload.ToBytes());
            data.AddRange(BitConverter.GetBytes(CRC16));
            return data.ToArray();
        }
    }
}
