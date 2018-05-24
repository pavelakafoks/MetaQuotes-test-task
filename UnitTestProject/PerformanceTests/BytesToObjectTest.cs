using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Engine.Geobase.Dirrect;
using Engine.Geobase.Marshal;
using Engine.Geobase.Marshal.DataDescription;
using Engine.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.PerformanceTests
{
    [TestClass]
    public class BytesToObjectTest
    {
        [TestMethod]
        public void PerformanceTest()
        {
            var count = 50000;

            var stopwatch = Stopwatch.StartNew();

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Data\\geobase.dat");
            var engineDirrect = new GeobaseEngineDirrect(fileName);

            var offset = (int)engineDirrect.Header.OffsetLocations;
            var resultsMarshal = new List<GeobaseLocationView>();
            for(int i = 0; i < count; i++)
            {
                var toAdd = ByteHelper.BytesToStruct<GeobaseLocationMarshal>(
                    new ArraySegment<byte>(
                        engineDirrect.Bytes, offset + i * engineDirrect.LocationLengh, engineDirrect.LocationLengh).ToArray()
                );
                resultsMarshal.Add(TransformationHelper.GetLocationView(toAdd));
            }

            stopwatch.Stop();
            Console.WriteLine("BytesToObjectTest.PerformanceTest - get locations from bytes, marshal access time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
            stopwatch.Restart();

            var resultsDirrect = new List<GeobaseLocationView>();

            for(int i = 0; i < count; i++)
            {
                resultsDirrect.Add(ByteHelper.GetLocation(engineDirrect.Bytes, offset + i * engineDirrect.LocationLengh));
            }

            stopwatch.Stop();
            Console.WriteLine("BytesToObjectTest.PerformanceTest - get locations from bytes, dirrect access time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
            stopwatch.Restart();

            Assert.AreEqual(resultsDirrect.Count,count);
            Assert.AreEqual(resultsMarshal.Count,count);

            Assert.AreEqual(resultsDirrect[0].city, resultsMarshal[0].city);
            Assert.AreEqual(resultsDirrect[0].country, resultsMarshal[0].country);
            Assert.AreEqual(resultsDirrect[0].latitude, resultsMarshal[0].latitude);
            Assert.AreEqual(resultsDirrect[0].longitude, resultsMarshal[0].longitude);
            Assert.AreEqual(resultsDirrect[0].organization, resultsMarshal[0].organization);
            Assert.AreEqual(resultsDirrect[0].postal, resultsMarshal[0].postal);
            Assert.AreEqual(resultsDirrect[0].region, resultsMarshal[0].region);

            var index = count - 10;
            Assert.AreEqual(resultsDirrect[index].city, resultsMarshal[index].city);
            Assert.AreEqual(resultsDirrect[index].country, resultsMarshal[index].country);
            Assert.AreEqual(resultsDirrect[index].latitude, resultsMarshal[index].latitude);
            Assert.AreEqual(resultsDirrect[index].longitude, resultsMarshal[index].longitude);
            Assert.AreEqual(resultsDirrect[index].organization, resultsMarshal[index].organization);
            Assert.AreEqual(resultsDirrect[index].postal, resultsMarshal[index].postal);
            Assert.AreEqual(resultsDirrect[index].region, resultsMarshal[index].region);

            index = count - 1;
            Assert.AreEqual(resultsDirrect[index].city, resultsMarshal[index].city);
            Assert.AreEqual(resultsDirrect[index].country, resultsMarshal[index].country);
            Assert.AreEqual(resultsDirrect[index].latitude, resultsMarshal[index].latitude);
            Assert.AreEqual(resultsDirrect[index].longitude, resultsMarshal[index].longitude);
            Assert.AreEqual(resultsDirrect[index].organization, resultsMarshal[index].organization);
            Assert.AreEqual(resultsDirrect[index].postal, resultsMarshal[index].postal);
            Assert.AreEqual(resultsDirrect[index].region, resultsMarshal[index].region);
        }
    }
}
