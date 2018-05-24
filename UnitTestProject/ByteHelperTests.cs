using System;
using System.Text;
using Engine.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTestProject
{
    [TestClass]
    public class ByteHelperTests
    {
        [TestMethod]
        public void Test_ByteHelper_CompareStringsAsBytes()
        {
            var strA = Encoding.ASCII.GetBytes("111");
            var strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes("222"));
            var comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);
            Assert.IsTrue( comparisonResult < 0);

            strA = Encoding.ASCII.GetBytes("ZZZ");
            strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes("ZZA"));
            comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);
            Assert.IsTrue(comparisonResult > 0);

            strA = Encoding.ASCII.GetBytes("MMM");
            strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes("MMM"));
            comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);
            Assert.IsTrue(comparisonResult == 0);

            strA = Encoding.ASCII.GetBytes("");
            strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes(""));
            comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);
            Assert.IsTrue(comparisonResult == 0);

            strA = Encoding.ASCII.GetBytes("   ");
            strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes("   "));
            comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);
            Assert.IsTrue(comparisonResult == 0);

            strA = Encoding.ASCII.GetBytes("KKK");
            strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes("KKK\0\0\0"));
            comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);
            Assert.IsTrue(comparisonResult == 0);

            strA = Encoding.ASCII.GetBytes("KKK_aaa");
            strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes("KKK\0\0\0"));
            comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);
            Assert.IsTrue(comparisonResult > 0);

            strA = Encoding.ASCII.GetBytes("KK");
            strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes("KKK\0\0\0"));
            comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);
            Assert.IsTrue(comparisonResult < 0);

            strA = Encoding.ASCII.GetBytes("09876");
            strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes("098"));
            comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);
            Assert.IsTrue(comparisonResult > 0);

            strA = Encoding.ASCII.GetBytes(" ");
            strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes("\0"));
            comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);
            Assert.IsTrue( comparisonResult > 0);
        }
    }
}
