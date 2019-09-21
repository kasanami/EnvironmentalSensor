using Ksnm.ExtensionMethods.System.Comparable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentalSensor
{
    /// <summary>
    /// センサーの値
    /// </summary>
    public class SensorValue
    {
        int raw;
        /// <summary>
        /// センサーの出力値そのまま
        /// </summary>
        public int Raw
        {
            get => raw;
            set
            {
                raw = value;
                Value = raw * Unit;
                Value = Value.Clamp(Min, Max);
            }
        }
        /// <summary>
        /// 単位
        /// </summary>
        public double Unit { get; private set; }
        /// <summary>
        /// 実用的な値
        /// </summary>
        public double Value { get; private set; }
        /// <summary>
        /// Valueの最小値
        /// </summary>
        public double Min { get; private set; }
        /// <summary>
        /// Valueの最大値
        /// </summary>
        public double Max { get; private set; }
        /// <summary>
        /// 記号
        /// </summary>
        public string Symbol { get; private set; }

        public SensorValue(int raw, double unit, double min, double max, string symbol)
        {
            Raw = raw;
            Unit = unit;
            Min = min;
            Max = max;
            Symbol = symbol;
        }
        #region override Object
        public override string ToString()
        {
            return Value.ToString() + Symbol;
        }
        #endregion override Object
    }
}
