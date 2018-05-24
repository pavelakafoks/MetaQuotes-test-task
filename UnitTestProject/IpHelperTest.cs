using Engine.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class IpHelperTest
    {
        [TestMethod]
        public void Test_IpStringToUint()
        {
            Assert.AreEqual(IpHelper.IpStringToUint("0.0.0.8"), (uint)8);
            Assert.AreEqual(IpHelper.IpStringToUint("0.0.216.176"), (uint)55472);
            Assert.AreEqual(IpHelper.IpStringToUint("0.0.216.174"), (uint)55470);
            Assert.AreEqual(IpHelper.IpStringToUint("0.145.243.27"), (uint)9564955);
            Assert.AreEqual(IpHelper.IpStringToUint("156.2.132.92"), (uint)2617410652);
            Assert.AreEqual(IpHelper.IpStringToUint("169.156.118.75"), (uint)2845603403);
            Assert.AreEqual(IpHelper.IpStringToUint("219.14.196.94"), (uint)3675178078);
            Assert.AreEqual(IpHelper.IpStringToUint("231.175.71.65"), (uint)3887023937);
            Assert.AreEqual(IpHelper.IpStringToUint("231.176.8.209"), (uint)3887073489);
            Assert.AreEqual(IpHelper.IpStringToUint("231.176.8.210"), (uint)3887073490);
        }

        [TestMethod]
        public void Test_IpUintToString()
        {
            Assert.AreEqual(IpHelper.IpUintToString(8), "0.0.0.8");
            Assert.AreEqual(IpHelper.IpUintToString(55472), "0.0.216.176");
            Assert.AreEqual(IpHelper.IpUintToString(55470), "0.0.216.174");
            Assert.AreEqual(IpHelper.IpUintToString(9564955), "0.145.243.27");
            Assert.AreEqual(IpHelper.IpUintToString(2617410652), "156.2.132.92");
            Assert.AreEqual(IpHelper.IpUintToString(2845603403), "169.156.118.75");
            Assert.AreEqual(IpHelper.IpUintToString(3675178078), "219.14.196.94");
            Assert.AreEqual(IpHelper.IpUintToString(3887023937), "231.175.71.65");
            Assert.AreEqual(IpHelper.IpUintToString(3887073489), "231.176.8.209");
            Assert.AreEqual(IpHelper.IpUintToString(3887073490), "231.176.8.210");
        }
    }
}
