using EnvironmentalSensor.USB;
using System;

namespace EnvironmentalSensor.Ipc
{
    public class RemoteObject : MarshalByRefObject
    {
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
        public int Index { get; protected set; }
        /// <summary>
        /// センサーから受信した情報
        /// </summary>
        public FramePayload[] Payloads { get; protected set; } = new FramePayload[256];
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
            Index++;
            if (Index >= Payloads.Length) { Index = 0; }
            Payloads[Index] = payload;
            UpdateCompleted = true;
        }
    }
}
