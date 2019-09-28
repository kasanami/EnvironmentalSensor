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
        /// Payloadsの最大数
        /// </summary>
        public const int PayloadsMaxCount = 1024;
        /// <summary>
        /// 情報を設定した日時
        /// </summary>
        public long TimeStampBinary;
        /// <summary>
        /// 情報を設定した日時
        /// </summary>
        public DateTime TimeStamp { get => DateTime.FromBinary(TimeStampBinary); }
        /// <summary>
        /// 受信データ
        /// </summary>
        public Dictionary<long, byte[]> ReceivedData { get; protected set; } = new Dictionary<long, byte[]>();
        /// <summary>
        /// センサーから受信した情報
        /// <para>最大値に達したら古いものから削除される</para>
        /// <para>Keyは、TimeStampBinary</para>
        /// </summary>
        //public Dictionary<long, FramePayload> Payloads { get; protected set; } = new Dictionary<long, FramePayload>();
        /// <summary>
        /// 情報の更新が完了しているならtrue
        /// <para>情報の更新直前にfalseに設定される</para>
        /// <para>更新途中にクライアントから参照するときにチェックする</para>
        /// </summary>
        public bool UpdateCompleted { get; protected set; }
        /// <summary>
        /// 情報を設定
        /// </summary>
        public void Set(DateTime now, byte[] receivedData, int count)
        {
            var buffer = new byte[count];
            Buffer.BlockCopy(receivedData, 0, buffer, 0, count);
            Set(now, buffer);
        }
        /// <summary>
        /// 情報を設定
        /// </summary>
        public void Set(DateTime now, byte[] receivedData)
        {
            UpdateCompleted = false;

#if DEBUG
            // わざと遅延させてデータ更新中にアクセスしていないかチェックしやすくする
            Thread.Sleep(100);
#endif

            TimeStampBinary = now.ToBinary();
            while (ReceivedData.ContainsKey(TimeStampBinary))
            {
                TimeStampBinary++;
            }
            ReceivedData.Add(TimeStampBinary, receivedData);
            while (ReceivedData.Count > PayloadsMaxCount)
            {
                var oldestKey = ReceivedData.Keys.Min();
                ReceivedData.Remove(oldestKey);
            }

            UpdateCompleted = true;
        }
        public byte[] GetReceivedData(long timeStampBinary)
        {
            var buffer = (byte[])ReceivedData[timeStampBinary].Clone();
            return buffer;
        }
#if false
        /// <summary>
        /// 情報を設定
        /// </summary>
        public void Set(FramePayload payload)
        {
            UpdateCompleted = false;

#if DEBUG
            // わざと遅延させてデータ更新中にアクセスしていないかチェックしやすくする
            Thread.Sleep(100);
#endif

            var now = DateTime.Now;

            TimeStampBinary = now.ToBinary();

            Payloads.Add(TimeStampBinary, payload);
            if (Payloads.Count > PayloadsMaxCount)
            {
                var oldestKey = Payloads.Keys.Min();
                Payloads.Remove(oldestKey);
            }

            UpdateCompleted = true;
        }
#endif
    }
}
