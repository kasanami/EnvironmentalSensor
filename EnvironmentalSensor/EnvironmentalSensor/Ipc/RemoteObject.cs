using EnvironmentalSensor.USB;
using EnvironmentalSensor.USB.Payloads;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EnvironmentalSensor.Ipc
{
    /// <summary>
    /// IPCで共有する情報
    /// </summary>
    public class RemoteObject : MarshalByRefObject
    {
        /// <summary>
        /// Payloadsの最大数
        /// </summary>
        const int PayloadsMaxCount = 256;
        /// <summary>
        /// 情報を設定した日時
        /// </summary>
        public long TimeStampBinary;
        /// <summary>
        /// 情報を設定した日時
        /// </summary>
        public DateTime TimeStamp { get => DateTime.FromBinary(TimeStampBinary); }
        /// <summary>
        /// センサーから受信した情報
        /// <para>最大値に達したら古いものから削除される</para>
        /// <para>Keyは、TimeStampBinary</para>
        /// </summary>
        public Dictionary<long, FramePayload> Payloads { get; protected set; } = new Dictionary<long, FramePayload>();
        /// <summary>
        /// 情報の更新が完了しているならtrue
        /// <para>情報の更新直前にfalseに設定される</para>
        /// <para>更新途中にクライアントから参照するときにチェックする</para>
        /// </summary>
        public bool UpdateCompleted { get; protected set; }
        /// <summary>
        /// 情報を設定
        /// </summary>
        public void Set(FramePayload payload)
        {
            UpdateCompleted = false;

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
    }
}
