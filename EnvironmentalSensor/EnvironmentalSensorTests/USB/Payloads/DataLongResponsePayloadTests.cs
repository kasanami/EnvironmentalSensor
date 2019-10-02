using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EnvironmentalSensor.Usb.Payloads.Tests
{
    [TestClass()]
    public class DataLongResponsePayloadTests
    {
        [TestMethod()]
        public void FormatTest()
        {
#if false
            // マニュアル:Table 83 Read response format
            var payload = new DataLongResponsePayload();
            Assert.AreEqual(typeof(Int16), payload.Temperature.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Temperature SInt16
            Assert.AreEqual(typeof(Int16), payload.RelativeHumidity.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Relative humidity SInt16
            Assert.AreEqual(typeof(Int16), payload.AmbientLight.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Ambient light SInt16
            Assert.AreEqual(typeof(Int32), payload.BarometricPressure.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Barometric pressure SInt32
            Assert.AreEqual(typeof(Int16), payload.SoundNoise.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Sound noise SInt16
            Assert.AreEqual(typeof(Int16), payload.eTVOC.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//eTVOC SInt16
            Assert.AreEqual(typeof(Int16), payload.eCO2.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//eCO2 SInt16
            Assert.AreEqual(typeof(Int16), payload.DiscomfortIndex.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Discomfort index SInt16
            Assert.AreEqual(typeof(Int16), payload.HeatStroke.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Heat stroke SInt16
            Assert.AreEqual(typeof(byte), payload.VibrationInformation.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Vibration information UInt8
            Assert.AreEqual(typeof(UInt16), payload.SIValue.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//SI value UInt16
            Assert.AreEqual(typeof(UInt16), payload.PGA.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//PGA UInt16
            Assert.AreEqual(typeof(UInt16), payload.SeismicIntensity.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Seismic intensity UInt16
            Assert.AreEqual(typeof(UInt16), payload.TemperatureFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Temperature flag UInt16
            Assert.AreEqual(typeof(UInt16), payload.RelativeHumidityFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Relative humidity flag UInt16
            Assert.AreEqual(typeof(UInt16), payload.AmbientLightFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Ambient light flag UInt16
            Assert.AreEqual(typeof(UInt16), payload.BarometricPressureFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Barometric pressure flag UInt16
            Assert.AreEqual(typeof(UInt16), payload.SoundNoiseFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Sound noise flag UInt16
            Assert.AreEqual(typeof(UInt16), payload.eTVOCFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//eTVOC flag UInt16
            Assert.AreEqual(typeof(UInt16), payload.eCO2Flag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//eCO2 flag UInt16
            Assert.AreEqual(typeof(UInt16), payload.DiscomfortIndexFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Discomfort index flag UInt16
            Assert.AreEqual(typeof(UInt16), payload.HeatStrokeFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Heat stroke flag UInt16
            Assert.AreEqual(typeof(byte), payload.SIValueFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//SI value flag UInt8
            Assert.AreEqual(typeof(byte), payload.PGAFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//PGA flag UInt8
            Assert.AreEqual(typeof(byte), payload.SeismicIntensityFlag.GetType(), Ksnm.Debug.GetFilePathAndLineNumber());//Seismic intensity flag UInt8
#endif
        }
    }
}
