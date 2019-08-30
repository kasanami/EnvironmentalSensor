using System;

namespace EnvironmentalSensor.USB.Payloads
{
    /// <summary>
    /// LatestDataLongResponsePayload,MemoryDataLongResponsePayloadの基底クラス
    /// プロパティを持つのみ、初期化機能はない
    /// </summary>
    public class DataLongResponsePayload : ResponsePayload
    {
        public DataLongResponsePayload(byte[] buffer, int index) : base(buffer, index)
        {
        }
        #region 各種センサの出力
        /// <summary>
        /// 気温
        /// 範囲:-40.00 to 125.00
        /// 単位:0.01 degC
        /// </summary>
        public Int16 Temperature { get; protected set; }
        /// <summary>
        /// 相対湿度
        /// 範囲:0.00 to 100.00
        /// 単位:0.01 %RH
        /// </summary>
        public Int16 RelativeHumidity { get; protected set; }
        /// <summary>
        /// 周囲光
        /// 範囲:0 to 30000
        /// 単位:1 lx
        /// </summary>
        public Int16 AmbientLight { get; protected set; }
        /// <summary>
        /// 範囲:300.000 to 1100.000
        /// 単位:0.001 hPa
        /// </summary>
        public Int32 BarometricPressure { get; protected set; }
        /// <summary>
        /// 範囲:33.00 to 120.00
        /// 単位:0.01 dB
        /// </summary>
        public Int16 SoundNoise { get; protected set; }
        /// <summary>
        /// equivalent Total Volatile Organic Compound
        /// 範囲:0 to 32767
        /// 単位:1 ppb
        /// </summary>
        public Int16 eTVOC { get; protected set; }
        /// <summary>
        /// equivalent CO2
        /// 範囲:400 to 32767
        /// 単位:1 ppm
        /// </summary>
        public Int16 eCO2 { get; protected set; }
        /// <summary>
        /// 夏の蒸し暑さを数量的に表現したもの．温度と湿度から換算する．
        /// 範囲:0.00 to 100.00
        /// 単位:0.01
        /// </summary>
        public Int16 DiscomfortIndex { get; protected set; }
        /// <summary>
        /// 熱中症の危険度を数量的に表現したもの．温度と湿度から換算する．
        /// 範囲:-40.00 to 125.00
        /// 単位:0.01 degC
        /// </summary>
        public Int16 HeatStroke { get; protected set; }

        public Byte VibrationInformation { get; protected set; }
        public UInt16 SIValue { get; protected set; }
        /// <summary>
        /// ある区間の最大加速度値．水平 2 軸の加速度値を合成して換算する．
        /// 範囲:0.0 to 6553.5 
        /// 単位:0.1 gal
        /// </summary>
        public UInt16 PGA { get; protected set; }
        /// <summary>
        /// SI 値から求めた震度に相関した値．
        /// 範囲:0.000 to 65.535
        /// 単位:0.001
        /// </summary>
        public UInt16 SeismicIntensity { get; protected set; }
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
        public Byte ValueFlag { get; protected set; }
        public Byte PGAFlag { get; protected set; }
        public Byte SeismicIntensityFlag { get; protected set; }
        #endregion フラグ
    }
}
