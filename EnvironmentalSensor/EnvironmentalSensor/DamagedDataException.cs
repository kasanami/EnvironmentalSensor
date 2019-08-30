using System;

namespace EnvironmentalSensor
{
    /// <summary>
    /// データが破損していることを示す例外
    /// </summary>
    public class DamagedDataException : Exception
    {
        public DamagedDataException()
        {
        }
        public DamagedDataException(string message) : base(message)
        {
        }
    }
}
