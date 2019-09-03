using System;
using System.IO;
using System.Text;
using Ksnm.ExtensionMethods.System.IO.Stream;

namespace EnvironmentalSensor.USB.Payloads
{
    public class MemoryDataLongResponsePayload : DataLongResponsePayload
    {
        #region Dataの内容
        /// <summary>
        /// Range: 0x00000001 to 0x7FFFFFFF
        /// If data error, MSB is 1
        /// </summary>
        public UInt32 MemoryIndex { get; private set; }
        /// <summary>
        /// Range: 0x1 to 0xFFFFFFFFFFFFFFFF
        /// </summary>
        public UInt64 TimeCounter { get; private set; }
        /// <summary>
        /// エラー
        /// </summary>
        public bool IsError { get => (MemoryIndex & 0x800_0000) != 0; }
        #endregion Dataの内容
        /// <summary>
        /// 指定されたバッファから初期化
        /// </summary>
        /// <param name="buffer">Payloadの範囲のバッファ</param>
        public MemoryDataLongResponsePayload(byte[] buffer, int index) : base(buffer, index)
        {
            var memoryStream = new MemoryStream(Data, 0, Data.Length, false);
            MemoryIndex = memoryStream.ReadUInt32();
            TimeCounter = memoryStream.ReadUInt16();
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
            text.AppendLine(nameof(MemoryIndex) + "=" + MemoryIndex);
            text.AppendLine(nameof(TimeCounter) + "=" + TimeCounter);
            text.AppendLine(nameof(IsError) + "=" + IsError);
            text.AppendLine(nameof(Temperature) + "=" + (Temperature * 0.01) + "degC");
            text.AppendLine(nameof(RelativeHumidity) + "=" + (RelativeHumidity * 0.01) + "%RH");
            text.AppendLine(nameof(AmbientLight) + "=" + (AmbientLight) + "lx");
            text.AppendLine(nameof(BarometricPressure) + "=" + (BarometricPressure * 0.001) + "hPa");
            text.AppendLine(nameof(SoundNoise) + "=" + (SoundNoise * 0.01) + "dB");
            text.AppendLine(nameof(eTVOC) + "=" + (eTVOC) + "ppb");
            text.AppendLine(nameof(eCO2) + "=" + (eCO2) + "ppm");
            text.AppendLine(nameof(DiscomfortIndex) + "=" + (DiscomfortIndex * 0.01));
            text.AppendLine(nameof(HeatStroke) + "=" + (HeatStroke * 0.01) + "degC");
            text.AppendLine(nameof(VibrationInformation) + "=" + (VibrationInformation));
            text.AppendLine(nameof(SIValue) + "=" + (SIValue));
            text.AppendLine(nameof(PGA) + "=" + (PGA * 0.1) + "gal");
            text.AppendLine(nameof(SeismicIntensity) + "=" + (SeismicIntensity * 0.001));
            text.AppendLine(nameof(TemperatureFlag) + "=" + TemperatureFlag);
            text.AppendLine(nameof(RelativeHumidityFlag) + "=" + RelativeHumidityFlag);
            text.AppendLine(nameof(AmbientLightFlag) + "=" + AmbientLightFlag);
            text.AppendLine(nameof(BarometricPressureFlag) + "=" + BarometricPressureFlag);
            text.AppendLine(nameof(SoundNoiseFlag) + "=" + SoundNoiseFlag);
            text.AppendLine(nameof(eTVOCFlag) + "=" + eTVOCFlag);
            text.AppendLine(nameof(eCO2Flag) + "=" + eCO2Flag);
            text.AppendLine(nameof(DiscomfortIndexFlag) + "=" + DiscomfortIndexFlag);
            text.AppendLine(nameof(HeatStrokeFlag) + "=" + HeatStrokeFlag);
            text.AppendLine(nameof(ValueFlag) + "=" + ValueFlag);
            text.AppendLine(nameof(PGAFlag) + "=" + PGAFlag);
            text.AppendLine(nameof(SeismicIntensityFlag) + "=" + SeismicIntensityFlag);
            text.AppendLine("}");
            return text.ToString();
        }
    }
}
