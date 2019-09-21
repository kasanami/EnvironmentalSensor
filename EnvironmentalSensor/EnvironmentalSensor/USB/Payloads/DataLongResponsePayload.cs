using System;
using System.IO;
using System.Text;

namespace EnvironmentalSensor.USB.Payloads
{
    /// <summary>
    /// LatestDataLongResponsePayload,MemoryDataLongResponsePayloadの基底クラス
    /// </summary>
    public class DataLongResponsePayload : ResponsePayload
    {
        /// <summary>
        /// 指定のストリームから初期化
        /// </summary>
        /// <param name="binaryReader">Payloadが含まれるストリーム</param>
        /// <param name="offset">Payloadの先頭位置のオフセット</param>
        public DataLongResponsePayload(BinaryReader binaryReader, int offset) : base(binaryReader, offset)
        {
            if (Address == FrameAddress.LatestDataLong)
            {
                binaryReader.BaseStream.Seek(offset + 1, SeekOrigin.Begin);
            }
            else if (Address == FrameAddress.MemoryDataLong)
            {
                binaryReader.BaseStream.Seek(offset + 6, SeekOrigin.Begin);
            }
            else
            {
                throw new NotImplementedException($"{nameof(Address)}={Address}");
            }
            Temperature.Raw = binaryReader.ReadInt16();
            RelativeHumidity.Raw = binaryReader.ReadInt16();
            AmbientLight.Raw = binaryReader.ReadInt16();
            BarometricPressure.Raw = binaryReader.ReadInt32();
            SoundNoise.Raw = binaryReader.ReadInt16();
            eTVOC.Raw = binaryReader.ReadInt16();
            eCO2.Raw = binaryReader.ReadInt16();
            DiscomfortIndex.Raw = binaryReader.ReadInt16();
            HeatStroke.Raw = binaryReader.ReadInt16();
            VibrationInformation = binaryReader.ReadByte();
            SIValue.Raw = binaryReader.ReadUInt16();
            PGA.Raw = binaryReader.ReadUInt16();
            SeismicIntensity.Raw = binaryReader.ReadUInt16();
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
        public DataLongResponsePayload()
        {
        }
        #region 各種センサの出力
        /// <summary>
        /// 気温
        /// <para>範囲:-40.00 to 125.00</para>
        /// <para>単位:0.01 degC</para>
        /// </summary>
        public SensorValue Temperature { get; protected set; } = new SensorValue(0, 0.01, -40, 125, "degC");
        /// <summary>
        /// 相対湿度
        /// <para>範囲:0.00 to 100.00</para>
        /// <para>単位:0.01 %RH</para>
        /// </summary>
        public SensorValue RelativeHumidity { get; protected set; } = new SensorValue(0, 0.01, 0, 100, "%RH");
        /// <summary>
        /// 周囲光
        /// <para>範囲:0 to 30000</para>
        /// <para>単位:1 lx(ルクス)</para>
        /// </summary>
        public SensorValue AmbientLight { get; protected set; } = new SensorValue(0, 1, 0, 30000, "lx");
        /// <summary>
        /// 気圧
        /// <para>範囲:300.000 to 1100.000</para>
        /// <para>単位:0.001 hPa</para>
        /// </summary>
        public SensorValue BarometricPressure { get; protected set; } = new SensorValue(0, 0.001, 300, 1100, "hPa");
        /// <summary>
        /// 雑音
        /// <para>範囲:33.00 to 120.00</para>
        /// <para>単位:0.01 dB</para>
        /// </summary>
        public SensorValue SoundNoise { get; protected set; } = new SensorValue(0, 0.01, 33, 120, "dB");
        /// <summary>
        /// 総揮発性有機化学物量相当値（equivalent Total Volatile Organic Compounds）の略称
        /// <para>範囲:0 to 32767</para>
        /// <para>単位:1 ppb</para>
        /// </summary>
        public SensorValue eTVOC { get; protected set; } = new SensorValue(0, 1, 0, 32767, "ppb");
        /// <summary>
        /// 二酸化炭素換算の数値(equivalent CO2)
        /// <para>範囲:400 to 32767</para>
        /// <para>単位:1 ppm</para>
        /// </summary>
        public SensorValue eCO2 { get; protected set; } = new SensorValue(0, 1, 400, 32767, "ppm");
        /// <summary>
        /// 不快指数
        /// <para>夏の蒸し暑さを数量的に表現したもの．温度と湿度から換算する．</para>
        /// <para>範囲:0.00 to 100.00</para>
        /// <para>単位:0.01</para>
        /// </summary>
        public SensorValue DiscomfortIndex { get; protected set; } = new SensorValue(0, 0.01, 0, 100, "");
        /// <summary>
        /// 熱中症警戒度
        /// <para>熱中症の危険度を数量的に表現したもの．温度と湿度から換算する．</para>
        /// <para>WEBサイト:熱中症に関する情報や不快指数はあくまで空調や体調管理の目安です。個人差、体調によって感じ方が大きく異なる場合があります。センサの出力により症状の発生有無を断定するものではありません。あくまで目安としてご利用ください。公的機関から発表される熱中症の警戒度とは一致しない場合があります。</para>
        /// <para>範囲:-40.00 to 125.00</para>
        /// <para>単位:0.01 degC</para>
        /// </summary>
        public SensorValue HeatStroke { get; protected set; } = new SensorValue(0, 0.01, -40, 125, "degC");
        /// <summary>
        /// 振動情報
        /// <para>0x00: NONE</para>
        /// <para>0x01: 振動中（地震判定中）during vibration(Earthquake judgment in progress)</para>
        /// <para>0x02: 地震中 during earthquake</para>
        /// </summary>
        public Byte VibrationInformation { get; protected set; }
        /// <summary>
        /// SI値：（スペクトル強度：Spectral Intensity）
        /// <para>WEBサイト:構造物に対する地震動の破壊エネルギーの大きさに相当します。SI値から震度相当値を算出でき、身近な環境の被害状況を把握できます</para>
        /// <para>範囲:0.0 to 6553.5</para>
        /// <para>単位:0.1 kine</para>
        /// </summary>
        public SensorValue SIValue { get; protected set; } = new SensorValue(0, 0.1, 0, 6553.5, "kine");
        /// <summary>
        /// ある区間の最大加速度値．水平 2 軸の加速度値を合成して換算する．
        /// <para>範囲:0.0 to 6553.5</para>
        /// <para>単位:0.1 gal</para>
        /// </summary>
        public SensorValue PGA { get; protected set; } = new SensorValue(0, 0.1, 0, 6553.5, "gal");
        /// <summary>
        /// SI 値から求めた震度に相関した値．
        /// <para>範囲:0.000 to 65.535</para>
        /// <para>単位:0.001</para>
        /// </summary>
        public SensorValue SeismicIntensity { get; protected set; } = new SensorValue(0, 0.001, 0, 65.535, "");
        #endregion 各種センサの出力

        #region フラグ
        public UInt16 TemperatureFlag { get; protected set; }
        public UInt16 RelativeHumidityFlag { get; protected set; }
        public UInt16 AmbientLightFlag { get; protected set; }
        public UInt16 BarometricPressureFlag { get; protected set; }
        public UInt16 SoundNoiseFlag { get; protected set; }
        public UInt16 eTVOCFlag { get; protected set; }
        public UInt16 eCO2Flag { get; protected set; }
        public UInt16 DiscomfortIndexFlag { get; protected set; }
        public UInt16 HeatStrokeFlag { get; protected set; }
        public Byte SIValueFlag { get; protected set; }
        public Byte PGAFlag { get; protected set; }
        public Byte SeismicIntensityFlag { get; protected set; }
        #endregion フラグ

        protected string ToInnerString()
        {
            var text = new StringBuilder();
            text.AppendLine(nameof(Temperature) + "=" + Temperature);
            text.AppendLine(nameof(RelativeHumidity) + "=" + RelativeHumidity);
            text.AppendLine(nameof(AmbientLight) + "=" + AmbientLight);
            text.AppendLine(nameof(BarometricPressure) + "=" + BarometricPressure);
            text.AppendLine(nameof(SoundNoise) + "=" + SoundNoise);
            text.AppendLine(nameof(eTVOC) + "=" + eTVOC);
            text.AppendLine(nameof(eCO2) + "=" + eCO2);
            text.AppendLine(nameof(DiscomfortIndex) + "=" + DiscomfortIndex);
            text.AppendLine(nameof(HeatStroke) + "=" + HeatStroke);
            text.AppendLine(nameof(VibrationInformation) + "=" + VibrationInformation);
            text.AppendLine(nameof(SIValue) + "=" + SIValue);
            text.AppendLine(nameof(PGA) + "=" + PGA);
            text.AppendLine(nameof(SeismicIntensity) + "=" + SeismicIntensity);
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
            return text.ToString();
        }
        /// <summary>
        /// ストリームに書き込む
        /// </summary>
        public void WriteOn(BinaryWriter binaryWriter)
        {
            binaryWriter.Write((Int16)Temperature.Raw);
            binaryWriter.Write((Int16)RelativeHumidity.Raw);
            binaryWriter.Write((Int16)AmbientLight.Raw);
            binaryWriter.Write((Int32)BarometricPressure.Raw);
            binaryWriter.Write((Int16)SoundNoise.Raw);
            binaryWriter.Write((Int16)eTVOC.Raw);
            binaryWriter.Write((Int16)eCO2.Raw);
            binaryWriter.Write((Int16)DiscomfortIndex.Raw);
            binaryWriter.Write((Int16)HeatStroke.Raw);
            binaryWriter.Write((byte)VibrationInformation);
            binaryWriter.Write((UInt16)SIValue.Raw);
            binaryWriter.Write((UInt16)PGA.Raw);
            binaryWriter.Write((UInt16)SeismicIntensity.Raw);

            binaryWriter.Write((UInt16)TemperatureFlag);
            binaryWriter.Write((UInt16)RelativeHumidityFlag);
            binaryWriter.Write((UInt16)AmbientLightFlag);
            binaryWriter.Write((UInt16)BarometricPressureFlag);
            binaryWriter.Write((UInt16)SoundNoiseFlag);
            binaryWriter.Write((UInt16)eTVOCFlag);
            binaryWriter.Write((UInt16)eCO2Flag);
            binaryWriter.Write((UInt16)DiscomfortIndexFlag);
            binaryWriter.Write((UInt16)HeatStrokeFlag);
            binaryWriter.Write((Byte)SIValueFlag);
            binaryWriter.Write((Byte)PGAFlag);
            binaryWriter.Write((Byte)SeismicIntensityFlag);
        }
    }
}
