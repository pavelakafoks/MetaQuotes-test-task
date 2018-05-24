using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Engine.Geobase;
using Engine.Geobase.Marshal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class GeobaseEngineMarchalTest
    {
        private IGeobaseEngine engineMarshal;

        [TestInitialize()]
        public void EngineDirrectInitialize()
        {
            var stopwatch = Stopwatch.StartNew();

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Data\\geobase.dat");
            engineMarshal = new GeobaseEngineMarshal(fileName);

            stopwatch.Stop();
            Console.WriteLine("Test_GeobaseEngineMarshal Init time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");

            Assert.IsNotNull(engineMarshal);
        }

        [TestMethod]
        public void Test_GeobaseEngineMarshal_FindLocationsByCity()
        {
            var stopwatch = Stopwatch.StartNew();
            GeobaseEngineTest.Test_FindLocationsByCity(engineMarshal);
            Console.WriteLine("Test_GeobaseEngineMarshal_FindLocationsByCity search time:  " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }

        [TestMethod]
        public void Test_GeobaseEngineMarshal_Test_FindLocationByIp()
        {
            var stopwatch = Stopwatch.StartNew();
            GeobaseEngineTest.Test_FindLocationByIp(engineMarshal);
            Console.WriteLine("Test_GeobaseEngineMarshal_Test_FindLocationByIp search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }


        [TestMethod]
        public void Test_GeobaseEngineMarshal_FindLocationsByCity_Multithreading()
        {
            var stopwatch = Stopwatch.StartNew();
            int tasksCount = 1000;

            var tasks = new Task[tasksCount];
            for (int i = 0; i < tasksCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => GeobaseEngineTest.Test_FindLocationsByCity(engineMarshal));
            }
            Task.WaitAll(tasks);

            Console.WriteLine("Test_GeobaseEngineMarshal_FindLocationsByCity_Multithreading search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }

        [TestMethod]
        public void Test_GeobaseEngineMarshal_Test_FindLocationByIp_Multithreading()
        {
            var stopwatch = Stopwatch.StartNew();
            int tasksCount = 1000;

            var tasks = new Task[tasksCount];
            for (int i = 0; i < tasksCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => GeobaseEngineTest.Test_FindLocationByIp(engineMarshal));
            }
            Task.WaitAll(tasks);

            Console.WriteLine("Test_GeobaseEngineMarshal_Test_FindLocationByIp_Multithreading search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}
