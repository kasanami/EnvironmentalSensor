namespace EnvironmentalSensor.Usb.Payloads
{
    /// <summary>
    /// 最新データを要求するペイロード
    /// </summary>
    public class LatestDataLongCommandPayload : CommandPayload
    {
        public override FrameCommand Command { get => FrameCommand.Read; }
        public override FrameAddress Address { get => FrameAddress.LatestDataLong; }
        /// <summary>
        /// Dataなし
        /// </summary>
        public override byte[] Data { get; set; } = new byte[0];
    }
}
