using EnvironmentalSensor.Usb.Payloads;
using Ksnm.Algorithm;
using Ksnm.ExtensionMethods.System.IO.Stream;
using System;
using System.Collections.Generic;
using System.IO;

namespace EnvironmentalSensor.Usb
{
    /// <summary>
    /// USB 通信の送受信で使用する共通フレーム
    /// マニュアル:4.3. Frame format
    /// </summary>
    public class Frame : IEquatable<Frame>
    {
        /// <summary>
        /// メンバーのHeader+Lengthのバイトサイズ
        /// </summary>
        const int TopSize = 4;
        /// <summary>
        /// メンバーのCRC16のバイトサイズ
        /// </summary>
        const int CRC16Size = 2;
        /// <summary>
        /// Headerに設定する値
        /// </summary>
        public const ushort MagicNumber = 0x4252;
        /// <summary>
        /// 最小サイズ（Header+Length+CRC）
        /// </summary>
        public const int MinimumSize = sizeof(ushort) + sizeof(ushort) + sizeof(ushort);
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
        static Crc16 crc16 = new Crc16(Crc16Polynomial.IbmReversed, 0xFFFF, 0);
        /// <summary>
        /// 指定のバッファから初期化
        /// </summary>
        /// <param name="buffer">フレームのバッファ</param>
        public Frame(byte[] buffer) : this(buffer, 0, buffer.Length) { }
        /// <summary>
        /// 指定のバッファから初期化
        /// </summary>
        /// <param name="buffer">フレームのバッファ</param>
        /// <param name="index">フレームの開始位置</param>
        /// <param name="count">フレームのバイト数</param>
        public Frame(byte[] buffer, int index, int count)
        {
            using (var memoryStream = new MemoryStream(buffer, index, count, false))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                // データ長チェック
                if (count < MinimumSize)
                {
                    throw new DamagedDataException($"データ長が 共通フレームの長さに満たない。{nameof(count)}={count} {nameof(MinimumSize)}={MinimumSize}");
                }
                // 
                Header = binaryReader.ReadUInt16();

                DebugWriteLine($"{nameof(Header)}={Header.ToString("X4")}");

                if (Header != MagicNumber)
                {
                    // 対応してるデータではない
                    throw new NotSupportedException($"Header={Header.ToString("X4")}");
                }
                Length = binaryReader.ReadUInt16();

                DebugWriteLine($"{nameof(Length)}={Length}");

                // データ長チェック
                {
                    // 本来あるべき全体のサイズ
                    var requestedLength = Length + TopSize;
                    if (count < requestedLength)
                    {
                        throw new DamagedDataException($"データ長が短い。{nameof(count)}={count} {nameof(requestedLength)}={requestedLength}");
                    }
                }
                // CRCチェック
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);// 最初に戻る
                    var tempBuffer = new byte[Length + TopSize - CRC16Size];
                    memoryStream.Read(tempBuffer, 0, tempBuffer.Length);
                    var computedCRC16 = crc16.ComputeHash(tempBuffer);
                    CRC16 = binaryReader.ReadUInt16();

                    DebugWriteLine($"{nameof(CRC16)}={CRC16}");

                    if (CRC16 != computedCRC16)
                    {
                        // CRCの結果 データ破損
                        throw new DamagedDataException($"CRC値の不一致。{nameof(CRC16)}={CRC16.ToString("X4")} {nameof(computedCRC16)}={computedCRC16.ToString("X4")}");
                    }
                }
                // Payload読み込み
                {
                    Payload = ResponsePayload.Create(binaryReader, TopSize);
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
            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(Header);
                binaryWriter.Write(Length);
                binaryWriter.Write(payloadBytes);
                CRC16 = crc16.ComputeHash(memoryStream.ToArray());
            }
        }
        /// <summary>
        /// バイト配列に変換する。
        /// </summary>
        /// <returns>バイト配列</returns>
        public byte[] ToBytes()
        {
            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(Header);
                binaryWriter.Write(Length);
                binaryWriter.Write(Payload.ToBytes());
                binaryWriter.Write(CRC16);
                return memoryStream.ToArray();
            }
        }
        /// <summary>
        /// デバッグ出力
        /// </summary>
        static void DebugWriteLine(string message)
        {
#if DEBUG
            //Console.WriteLine(message);
#endif
        }

        public static int GetLength(byte[] buffer)
        {
            const int TopSize = 4;
            const int CRC16Size = 2;
            using (var memoryStream = new MemoryStream(buffer, false))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                // データ長チェック
                {
                    var requestedLength = TopSize + CRC16Size;
                    if (buffer.Length < requestedLength)
                    {
                        throw new DamagedDataException($"データ長が 共通フレームの長さに満たない。{nameof(buffer.Length)}={buffer.Length} {nameof(requestedLength)}={requestedLength }");
                    }
                }
                var header = binaryReader.ReadUInt16();

                if (header != MagicNumber)
                {
                    // 対応してるデータではない
                    throw new NotSupportedException($"Header={header.ToString("X4")}");
                }
                return binaryReader.ReadUInt16();
            }
        }
        /// <summary>
        /// 現状のLength値からフレームのバイトサイズを計算
        /// </summary>
        /// <returns>計算したバイトサイズ</returns>
        public int GetSize()
        {
            return Length + TopSize;
        }
        /// <summary>
        /// バイト列からフレームのサイズを取得する。
        /// </summary>
        /// <param name="buffer">フレームのバッファ</param>
        public static int GetSize(byte[] buffer)
        {
            return GetSize(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// バイト列からフレームのサイズを取得する。
        /// </summary>
        /// <param name="buffer">フレームのバッファ</param>
        /// <param name="index">フレームの開始位置</param>
        /// <param name="count">フレームのバイト数</param>
        public static int GetSize(byte[] buffer, int index, int count)
        {
            using (var memoryStream = new MemoryStream(buffer, index, count, false))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                // データ長チェック
                if (count < MinimumSize)
                {
                    throw new DamagedDataException($"データ長が 共通フレームの長さに満たない。{nameof(count)}={count} {nameof(MinimumSize)}={MinimumSize}");
                }
                // 
                var header = binaryReader.ReadUInt16();
                if (header != MagicNumber)
                {
                    // 対応してるデータではない
                    throw new NotSupportedException($"{nameof(header)}={header:X4}");
                }
                var length = binaryReader.ReadUInt16();
                // データ長チェック
                {
                    // 本来あるべき全体のサイズ
                    var requestedLength = length + TopSize;
                    if (count < requestedLength)
                    {
                        throw new DamagedDataException($"データ長が短い。{nameof(count)}={count} {nameof(requestedLength)}={requestedLength}");
                    }
                }
                // CRCチェック
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);// 最初に戻る
                    var tempBuffer = new byte[length + TopSize - CRC16Size];
                    memoryStream.Read(tempBuffer, 0, tempBuffer.Length);
                    var computedCRC16 = crc16.ComputeHash(tempBuffer);
                    var readCRC16 = binaryReader.ReadUInt16();
                    if (readCRC16 != computedCRC16)
                    {
                        // CRCの結果 データ破損
                        throw new DamagedDataException($"CRC値の不一致。{nameof(readCRC16)}={readCRC16:X4} {nameof(computedCRC16)}={computedCRC16:X4}");
                    }
                }
                return length + TopSize;
            }
        }
        #region IEquatable
        public bool Equals(Frame other)
        {
            if (Header != other.Header) { return false; }
            if (Length != other.Length) { return false; }
            if (Payload.Equals(other.Payload) == false) { return false; }
            if (CRC16 != other.CRC16) { return false; }
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
            if (obj is Frame)
            {
                return Equals((Frame)obj);
            }
            return false;
        }
        /// <summary>
        /// このインスタンスのハッシュ コードを返します。
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = 0;
            hashCode ^= Header;
            hashCode ^= Length;
            hashCode ^= Payload.GetHashCode();
            hashCode ^= CRC16;
            return hashCode;
        }
        #endregion object
    }
}
