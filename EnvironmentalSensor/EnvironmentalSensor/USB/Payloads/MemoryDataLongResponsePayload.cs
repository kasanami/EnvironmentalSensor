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
        public MemoryDataLongResponsePayload(byte[] buffer) : base(buffer)
        {
            using (var memoryStream = new MemoryStream(Data, 0, Data.Length, false))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                MemoryIndex = binaryReader.ReadUInt32();
                TimeCounter = binaryReader.ReadUInt16();
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
            text.AppendLine(nameof(SIValueFlag) + "=" + SIValueFlag);
            text.AppendLine(nameof(PGAFlag) + "=" + PGAFlag);
            text.AppendLine(nameof(SeismicIntensityFlag) + "=" + SeismicIntensityFlag);
            text.AppendLine("}");
            return text.ToString();
        }
    }
}
