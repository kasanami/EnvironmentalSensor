using System;
using System.IO;
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
    }
}
