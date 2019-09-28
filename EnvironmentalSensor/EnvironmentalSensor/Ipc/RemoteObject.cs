using EnvironmentalSensor.USB;
using EnvironmentalSensor.USB.Payloads;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace EnvironmentalSensor.Ipc
{
    /// <summary>
    /// IPCで共有する情報
    /// </summary>
    public class RemoteObject : MarshalByRefObject
    {
        /// <summary>
        /// RemoteObject用Mutexの名前
        /// </summary>
        public const string MutexName = "Local/EnvironmentalSensor.Ipc.RemoteObject";
        /// <summary>
        /// 履歴の最大数
        /// </summary>
        public const int HistoryMaxCount = 1024;
        /// <summary>
        /// 情報を設定した日時
        /// </summary>
        public long TimeStampBinary;
        /// <summary>
        /// 情報を設定した日時
        /// </summary>
        public DateTime TimeStamp { get => DateTime.FromBinary(TimeStampBinary); }
        /// <summary>
        /// 受信データの履歴
        /// </summary>
        public Dictionary<long, byte[]> ReceivedDataHistory { get; protected set; } = new Dictionary<long, byte[]>();
        /// <summary>
        /// 情報の更新が完了しているならtrue
        /// <para>情報の更新直前にfalseに設定される</para>
        /// <para>更新途中にクライアントから参照するときのチェックに使用する</para>
        /// </summary>
        public bool UpdateCompleted { get; protected set; }
        /// <summary>
        /// 受信データを設定
        /// </summary>
        public void SetReceivedData(DateTime now, byte[] receivedData, int count)
        {
            var buffer = new byte[count];
            Buffer.BlockCopy(receivedData, 0, buffer, 0, count);
            SetReceivedData(now, buffer);
        }
        /// <summary>
        /// 受信データを設定
        /// </summary>
        public void SetReceivedData(DateTime now, byte[] receivedData)
        {
            // 更新開始
            UpdateCompleted = false;
#if DEBUG
            // わざと遅延させてデータ更新中にアクセスしていないかチェックしやすくする
            Thread.Sleep(100);
#endif
            //
            TimeStampBinary = now.ToBinary();
            // 連続で処理されると同じキー値になるので、少し時間をすすめる。
            // もともと実際の測定日時と受信日時にずれがあるので、多少は良しとする。
            while (ReceivedDataHistory.ContainsKey(TimeStampBinary))
            {
                TimeStampBinary++;
            }
            // 履歴に追加
            ReceivedDataHistory.Add(TimeStampBinary, receivedData);
            // 古い履歴を削除
            while (ReceivedDataHistory.Count > HistoryMaxCount)
            {
                var oldestKey = ReceivedDataHistory.Keys.Min();
                ReceivedDataHistory.Remove(oldestKey);
            }
            // 更新終了
            UpdateCompleted = true;
        }
        /// <summary>
        /// 受信データを取得
        /// </summary>
        /// <param name="timeStampBinary">取得したい履歴の日時（DateTimeをToBinaryした値）</param>
        /// <returns></returns>
        public byte[] GetReceivedData(long timeStampBinary)
        {
            var buffer = (byte[])ReceivedDataHistory[timeStampBinary].Clone();
            return buffer;
        }
    }
}
