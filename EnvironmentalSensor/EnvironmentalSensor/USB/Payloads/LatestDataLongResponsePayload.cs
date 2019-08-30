using System.IO;
using System.Text;
using Ksnm.ExtensionMethods.System.IO.Stream;
using UInt8 = System.Byte;

namespace EnvironmentalSensor.USB.Payloads
{
    /// <summary>
    /// 最新データの返答
    /// </summary>
    public class LatestDataLongResponsePayload : DataLongResponsePayload
    {
        #region Dataの内容
        /// <summary>
        /// ？
        /// Range: 0x00 to 0xFF
        /// </summary>
        public UInt8 SequenceNumber;
        #endregion Dataの内容
        /// <summary>
        /// 指定されたバッファから初期化
        /// </summary>
        /// <param name="buffer">Payloadの範囲のバッファ</param>
        public LatestDataLongResponsePayload(byte[] buffer, int index) : base(buffer, index)
        {
            var memoryStream = new MemoryStream(Data, 0, Data.Length, false);
            SequenceNumber = memoryStream.ReadUInt8();
            Temperature = memoryStream.ReadInt16();
            RelativeHumidity = memoryStream.ReadInt16();
            AmbientLight = memoryStream.ReadInt16();
            BarometricPressure = memoryStream.ReadInt16();
            SoundNoise = memoryStream.ReadInt16();
            eTVOC = memoryStream.ReadInt16();
            eCO2 = memoryStream.ReadInt16();
            DiscomfortIndex = memoryStream.ReadInt16();
            HeatStroke = memoryStream.ReadInt16();
            VibrationInformation = (byte)memoryStream.ReadByte();
            SIValue = memoryStream.ReadUInt16();
            PGA = memoryStream.ReadUInt16();
            SeismicIntensity = memoryStream.ReadUInt16();
            TemperatureFlag = memoryStream.ReadUInt16();
            RelativeHumidityFlag = memoryStream.ReadUInt16();
            AmbientLightFlag = memoryStream.ReadUInt16();
            BarometricPressureFlag = memoryStream.ReadUInt16();
            SoundNoiseFlag = memoryStream.ReadUInt16();
            eTVOCFlag = memoryStream.ReadUInt16();
            eCO2Flag = memoryStream.ReadUInt16();
            DiscomfortIndexFlag = memoryStream.ReadUInt16();
            HeatStrokeFlag = memoryStream.ReadUInt16();
            ValueFlag = (byte)memoryStream.ReadByte();
            PGAFlag = (byte)memoryStream.ReadByte();
            SeismicIntensityFlag = (byte)memoryStream.ReadByte();
        }

        public override string ToString()
        {
            var text = new StringBuilder();
            text.AppendLine("{");
            text.AppendLine(nameof(SequenceNumber) + "=" + SequenceNumber.ToString());
            text.AppendLine(nameof(Temperature) + "=" + (Temperature * 0.01).ToString() + "degC");
            text.AppendLine(nameof(RelativeHumidity) + "=" + (RelativeHumidity * 0.01).ToString() + "%RH");
            text.AppendLine(nameof(AmbientLight) + "=" + (AmbientLight).ToString() + "lx");
            text.AppendLine(nameof(BarometricPressure) + "=" + (BarometricPressure * 0.001).ToString() + "hPa");
            text.AppendLine(nameof(SoundNoise) + "=" + (SoundNoise * 0.01).ToString() + "dB");
            text.AppendLine(nameof(eTVOC) + "=" + (eTVOC).ToString() + "ppb");
            text.AppendLine(nameof(eCO2) + "=" + (eCO2).ToString() + "ppm");
            text.AppendLine(nameof(DiscomfortIndex) + "=" + (DiscomfortIndex * 0.01).ToString());
            text.AppendLine(nameof(HeatStroke) + "=" + (HeatStroke * 0.01).ToString() + "degC");
            text.AppendLine(nameof(VibrationInformation) + "=" + (VibrationInformation).ToString());
            text.AppendLine(nameof(SIValue) + "=" + (SIValue).ToString());
            text.AppendLine(nameof(PGA) + "=" + (PGA * 0.1).ToString() + "gal");
            text.AppendLine(nameof(SeismicIntensity) + "=" + (SeismicIntensity * 0.001).ToString());
            text.AppendLine(nameof(TemperatureFlag) + "=" + TemperatureFlag.ToString());
            text.AppendLine(nameof(RelativeHumidityFlag) + "=" + RelativeHumidityFlag.ToString());
            text.AppendLine(nameof(AmbientLightFlag) + "=" + AmbientLightFlag.ToString());
            text.AppendLine(nameof(BarometricPressureFlag) + "=" + BarometricPressureFlag.ToString());
            text.AppendLine(nameof(SoundNoiseFlag) + "=" + SoundNoiseFlag.ToString());
            text.AppendLine(nameof(eTVOCFlag) + "=" + eTVOCFlag.ToString());
            text.AppendLine(nameof(eCO2Flag) + "=" + eCO2Flag.ToString());
            text.AppendLine(nameof(DiscomfortIndexFlag) + "=" + DiscomfortIndexFlag.ToString());
            text.AppendLine(nameof(HeatStrokeFlag) + "=" + HeatStrokeFlag.ToString());
            text.AppendLine(nameof(ValueFlag) + "=" + ValueFlag.ToString());
            text.AppendLine(nameof(PGAFlag) + "=" + PGAFlag.ToString());
            text.AppendLine(nameof(SeismicIntensityFlag) + "=" + SeismicIntensityFlag.ToString());
            text.AppendLine("}");
            return text.ToString();
        }
    }
}
