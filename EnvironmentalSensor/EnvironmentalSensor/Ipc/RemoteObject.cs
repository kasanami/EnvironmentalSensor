using EnvironmentalSensor.USB;
using EnvironmentalSensor.USB.Payloads;
using System;
using System.Collections.Generic;

namespace EnvironmentalSensor.Ipc
{
    public class RemoteObject : MarshalByRefObject
    {
        /// <summary>
        /// Payloadsの最大数
        /// </summary>
        const int PayloadsMaxCount = 1024;
        /// <summary>
        /// 情報を設定した日時
        /// </summary>
        public long TimeStampTicks;
        /// <summary>
        /// 情報を設定した日時
        /// </summary>
        public DateTime TimeStamp { get => new DateTime(TimeStampTicks); }
        /// <summary>
        /// 最新のPayloadsのインデックス
        /// </summary>
        public int LatestIndex { get; protected set; }
        /// <summary>
        /// センサーから受信した情報
        /// <para>最大値に達したら古いものから削除される</para>
        /// </summary>
        public List<FramePayload> Payloads { get; protected set; } = new List<FramePayload>();
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

            TimeStampTicks = DateTime.Now.Ticks;

            Payloads.Add(payload);
            if (Payloads.Count > PayloadsMaxCount)
            {
                Payloads.RemoveAt(0);
            }
            LatestIndex = Payloads.Count - 1;

            UpdateCompleted = true;
        }
    }
}
