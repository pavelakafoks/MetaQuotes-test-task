using System;
using System.Diagnostics;
using System.Text;
using Engine.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class StringSpeedTest
    {
        [TestMethod]
        public void StringSpeedTest_Comparison()
        {
            var str1 = "111";
            var str2 = "222";
            var iterations = 1000000;

            var strA = Encoding.ASCII.GetBytes(str1);
            var strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes(str2));

            var stopwatch = Stopwatch.StartNew();

            int comparisonResult = 0;
            for (int i = 1; i < iterations; i++)
            {
                comparisonResult = ByteHelper.CompareStringsAsBytes(strA, strB);

                strA = Encoding.ASCII.GetBytes(str1 + i);
                strB = new ArraySegment<byte>(Encoding.ASCII.GetBytes(str2 + i));
            }

            stopwatch.Stop();
            Console.WriteLine("ByteHelper.CompareStringsAsBytes time: " + stopwatch.Elapsed.TotalMilliseconds + " ms., result: " + comparisonResult);
            stopwatch.Restart();

            for (int i = 1; i < iterations; i++)
            {
                comparisonResult = string.CompareOrdinal(str1 + i, str2 + i);
            }

            stopwatch.Stop();
            Console.WriteLine("string.CompareOrdinal time: " + stopwatch.Elapsed.TotalMilliseconds + " ms., result: " + comparisonResult);
            stopwatch.Restart();

            var strB2 = Encoding.ASCII.GetBytes(str2);

            for (int i = 1; i < iterations; i++)
            {
                comparisonResult = ByteHelper.CompareStrings(str1 + i, strB2, 3, 0);
                strB2 = Encoding.ASCII.GetBytes(str2 + i);
            }

            stopwatch.Stop();
            Console.WriteLine("ByteHelper.CompareStrings time: " + stopwatch.Elapsed.TotalMilliseconds + " ms., result: " + comparisonResult);
            stopwatch.Restart();
        }
    }
}
