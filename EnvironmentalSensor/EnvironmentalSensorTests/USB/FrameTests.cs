using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnvironmentalSensor.USB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentalSensor.USB.Tests
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
    }
}
