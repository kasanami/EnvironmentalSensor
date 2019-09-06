using System;

namespace EnvironmentalSensor.USB.Payloads
{
    /// <summary>
    /// LatestDataLongResponsePayload,MemoryDataLongResponsePayloadの基底クラス
    /// プロパティを持つのみ、初期化機能はない
    /// </summary>
    public class DataLongResponsePayload : ResponsePayload
    {
        public DataLongResponsePayload(byte[] buffer) : base(buffer)
        {
        }
        public DataLongResponsePayload()
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
        /// Temperature の単位を変換するための係数
        /// </summary>
        public double TemperatureUnit { get; } = 0.01;
        /// <summary>
        /// 相対湿度
        /// 範囲:0.00 to 100.00
        /// 単位:0.01 %RH
        /// </summary>
        public Int16 RelativeHumidity { get; protected set; }
        /// <summary>
        /// RelativeHumidity の単位を変換するための係数
        /// </summary>
        public double RelativeHumidityUnit { get; } = 0.01;
        /// <summary>
        /// 周囲光
        /// 範囲:0 to 30000
        /// 単位:1 lx(ルクス)
        /// </summary>
        public Int16 AmbientLight { get; protected set; }
        /// <summary>
        /// 気圧
        /// 範囲:300.000 to 1100.000
        /// 単位:0.001 hPa
        /// </summary>
        public Int32 BarometricPressure { get; protected set; }
        /// <summary>
        /// BarometricPressure の単位を変換するための係数
        /// </summary>
        public double BarometricPressureUnit { get; } = 0.001;
        /// <summary>
        /// 雑音
        /// 範囲:33.00 to 120.00
        /// 単位:0.01 dB
        /// </summary>
        public Int16 SoundNoise { get; protected set; }
        /// <summary>
        /// SoundNoise の単位を変換するための係数
        /// </summary>
        public double SoundNoiseUnit { get; } = 0.01;
        /// <summary>
        /// 総揮発性有機化学物量相当値（equivalent Total Volatile Organic Compounds）の略称
        /// 範囲:0 to 32767
        /// 単位:1 ppb
        /// </summary>
        public Int16 eTVOC { get; protected set; }
        /// <summary>
        /// 二酸化炭素換算の数値(equivalent CO2)
        /// 範囲:400 to 32767
        /// 単位:1 ppm
        /// </summary>
        public Int16 eCO2 { get; protected set; }
        /// <summary>
        /// 不快指数
        /// 夏の蒸し暑さを数量的に表現したもの．温度と湿度から換算する．
        /// 範囲:0.00 to 100.00
        /// 単位:0.01
        /// </summary>
        public Int16 DiscomfortIndex { get; protected set; }
        /// <summary>
        /// DiscomfortIndex の単位を変換するための係数
        /// </summary>
        public double DiscomfortIndexUnit { get; } = 0.01;
        /// <summary>
        /// 熱中症警戒度
        /// 熱中症の危険度を数量的に表現したもの．温度と湿度から換算する．
        /// WEBサイト:熱中症に関する情報や不快指数はあくまで空調や体調管理の目安です。個人差、体調によって感じ方が大きく異なる場合があります。センサの出力により症状の発生有無を断定するものではありません。あくまで目安としてご利用ください。公的機関から発表される熱中症の警戒度とは一致しない場合があります。
        /// 範囲:-40.00 to 125.00
        /// 単位:0.01 degC
        /// </summary>
        public Int16 HeatStroke { get; protected set; }
        /// <summary>
        /// HeatStroke の単位を変換するための係数
        /// </summary>
        public double HeatStrokeUnit { get; } = 0.01;
        /// <summary>
        /// 振動情報
        /// 0x00: NONE
        /// 0x01: 振動中（地震判定中）during vibration(Earthquake judgment in progress)
        /// 0x02: 地震中 during earthquake
        /// </summary>
        public Byte VibrationInformation { get; protected set; }
        /// <summary>
        /// SI値：（スペクトル強度：Spectral Intensity）
        /// WEBサイト:構造物に対する地震動の破壊エネルギーの大きさに相当します。SI値から震度相当値を算出でき、身近な環境の被害状況を把握できます
        /// 範囲:0.0 to 6553.5
        /// 単位:0.1 kine
        /// </summary>
        public UInt16 SIValue { get; protected set; }
        /// <summary>
        /// SIValue の単位を変換するための係数
        /// </summary>
        public double SIValueUnit { get; } = 0.1;
        /// <summary>
        /// ある区間の最大加速度値．水平 2 軸の加速度値を合成して換算する．
        /// 範囲:0.0 to 6553.5 
        /// 単位:0.1 gal
        /// </summary>
        public UInt16 PGA { get; protected set; }
        /// <summary>
        /// PGA の単位を変換するための係数
        /// </summary>
        public double PGAUnit { get; } = 0.1;
        /// <summary>
        /// SI 値から求めた震度に相関した値．
        /// 範囲:0.000 to 65.535
        /// 単位:0.001
        /// </summary>
        public UInt16 SeismicIntensity { get; protected set; }
        /// <summary>
        /// SeismicIntensity の単位を変換するための係数
        /// </summary>
        public double SeismicIntensityUnit { get; } = 0.001;
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
    }
}
