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
        public LatestDataLongResponsePayload(byte[] buffer) : base(buffer)
        {
            using (var memoryStream = new MemoryStream(Data, 0, Data.Length, false))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                SequenceNumber = binaryReader.ReadByte();
                Temperature = binaryReader.ReadInt16();
                RelativeHumidity = binaryReader.ReadInt16();
                AmbientLight = binaryReader.ReadInt16();
                BarometricPressure = binaryReader.ReadInt16();
                SoundNoise = binaryReader.ReadInt16();
                eTVOC = binaryReader.ReadInt16();
                eCO2 = binaryReader.ReadInt16();
                DiscomfortIndex = binaryReader.ReadInt16();
                HeatStroke = binaryReader.ReadInt16();
                VibrationInformation = binaryReader.ReadByte();
                SIValue = binaryReader.ReadUInt16();
                PGA = binaryReader.ReadUInt16();
                SeismicIntensity = binaryReader.ReadUInt16();
                TemperatureFlag = binaryReader.ReadUInt16();
                RelativeHumidityFlag = binaryReader.ReadUInt16();
                AmbientLightFlag = binaryReader.ReadUInt16();
                BarometricPressureFlag = binaryReader.ReadUInt16();
                SoundNoiseFlag = binaryReader.ReadUInt16();
                eTVOCFlag = binaryReader.ReadUInt16();
                eCO2Flag = binaryReader.ReadUInt16();
                DiscomfortIndexFlag = binaryReader.ReadUInt16();
                HeatStrokeFlag = binaryReader.ReadUInt16();
                SIValueFlag = binaryReader.ReadByte();
                PGAFlag = binaryReader.ReadByte();
                SeismicIntensityFlag = binaryReader.ReadByte();
            }
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
            text.AppendLine(nameof(SIValueFlag) + "=" + SIValueFlag.ToString());
            text.AppendLine(nameof(PGAFlag) + "=" + PGAFlag.ToString());
            text.AppendLine(nameof(SeismicIntensityFlag) + "=" + SeismicIntensityFlag.ToString());
            text.AppendLine("}");
            return text.ToString();
        }
    }
}
