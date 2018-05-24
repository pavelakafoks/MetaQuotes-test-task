using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Engine.Geobase;
using Engine.Geobase.Combined;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class GeobaseEngineCombinedTest
    {
        private IGeobaseEngine engineCombined;

        [TestInitialize]
        public void EngineCombinedInitialize()
        {
            var stopwatch = Stopwatch.StartNew();

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Data\\geobase.dat");
            engineCombined = new GeobaseEngineCombined(fileName);

            stopwatch.Stop();
            Console.WriteLine("Test_GeobaseEngineCombined Init time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");

            Assert.IsNotNull(engineCombined);
        }

        [TestMethod]
        public void Test_GeobaseEngineCombined_FindLocationsByCity()
        {
            var stopwatch = Stopwatch.StartNew();
            GeobaseEngineTest.Test_FindLocationsByCity(engineCombined);
            Console.WriteLine("Test_GeobaseEngineCombined_FindLocationsByCity search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }

        [TestMethod]
        public void Test_GeobaseEngineCombined_Test_FindLocationByIp()
        {
            var stopwatch = Stopwatch.StartNew();
            GeobaseEngineTest.Test_FindLocationByIp(engineCombined);
            Console.WriteLine("Test_GeobaseEngineCombined_Test_FindLocationByIp search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }



        [TestMethod]
        public void Test_GeobaseEngineCombined_FindLocationsByCity_Multithreading()
        {
            var stopwatch = Stopwatch.StartNew();
            int tasksCount = 1000;

            var tasks = new Task[tasksCount];
            for (int i = 0; i < tasksCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => GeobaseEngineTest.Test_FindLocationsByCity(engineCombined));
            }
            Task.WaitAll(tasks);

            Console.WriteLine("Test_GeobaseEngineCombined_FindLocationsByCity_Multithreading search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }

        [TestMethod]
        public void Test_GeobaseEngineCombined_Test_FindLocationByIp_Multithreading()
        {
            var stopwatch = Stopwatch.StartNew();
            int tasksCount = 1000;

            var tasks = new Task[tasksCount];
            for (int i = 0; i < tasksCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => GeobaseEngineTest.Test_FindLocationByIp(engineCombined));
            }
            Task.WaitAll(tasks);

            Console.WriteLine("Test_GeobaseEngineCombined_Test_FindLocationByIp_Multithreading search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}
