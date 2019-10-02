using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnvironmentalSensor.Usb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentalSensor.Usb.Tests
{
    [TestClass()]
    public class FrameTests
    {
        [TestMethod()]
        public void ToBytesTest()
        {
            var payload = new Payloads.LatestDataLongCommandPayload();
            var frame = new Frame(payload);
            var actualBytes = frame.ToBytes();
            var expectedBytes = new byte[] { 0x52, 0x42, 0x05, 0x00, 0x01, 0x21, 0x50, 0xE2, 0x4B };
            Assert.IsTrue(expectedBytes.SequenceEqual(actualBytes), Ksnm.Debug.GetFilePathAndLineNumber());
        }
        [TestMethod()]
        public void ToBytes2Test()
        {
            var payload = new Payloads.LatestDataLongResponsePayload(123);
            {
                payload.Temperature.Raw = 1;
                payload.RelativeHumidity.Raw = 2;
                payload.AmbientLight.Raw = 3;
                payload.BarometricPressure.Raw = 4;
                payload.SoundNoise.Raw = 5;
                payload.eTVOC.Raw = 6;
                payload.eCO2.Raw = 7;
                payload.DiscomfortIndex.Raw = 8;
                payload.HeatStroke.Raw = 9;
                //payload.VibrationInformation = 10;
                payload.SIValue.Raw = 11;
                payload.PGA.Raw = 12;
                payload.SeismicIntensity.Raw = 13;
                //payload.TemperatureFlag = 14;
                //payload.RelativeHumidityFlag = 15;
                //payload.AmbientLightFlag = 16;
                //payload.BarometricPressureFlag = 17;
                //payload.SoundNoiseFlag = 18;
                //payload.eTVOCFlag = 19;
                //payload.eCO2Flag = 20;
                //payload.DiscomfortIndexFlag = 21;
                //payload.HeatStrokeFlag = 22;
                //payload.SIValueFlag = 23;
                //payload.PGAFlag = 24;
                //payload.SeismicIntensityFlag = 25;
            }
            var frame = new Frame(payload);
            var bytes = frame.ToBytes();
            var frame2 = new Frame(bytes, 0, bytes.Length);
            var bytes2 = frame2.ToBytes();
            Assert.IsTrue(frame.Equals(frame2), Ksnm.Debug.GetFilePathAndLineNumber());
            Assert.IsTrue(bytes.SequenceEqual(bytes2), Ksnm.Debug.GetFilePathAndLineNumber());
        }
    }
}
