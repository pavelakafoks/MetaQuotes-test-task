using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Engine.Geobase;
using Engine.Geobase.Dirrect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class GeobaseEngineDirrectTest
    {
        private IGeobaseEngine engineDirrect;

        [TestInitialize]
        public void EngineDirrectInitialize()
        {
            var stopwatch = Stopwatch.StartNew();

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Data\\geobase.dat");
            engineDirrect = new GeobaseEngineDirrect(fileName);

            stopwatch.Stop();
            Console.WriteLine("Test_GeobaseEngineDirrect Init time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");

            Assert.IsNotNull(engineDirrect);
        }

        [TestMethod]
        public void Test_GeobaseEngineDirrect_FindLocationsByCity()
        {
            var stopwatch = Stopwatch.StartNew();
            GeobaseEngineTest.Test_FindLocationsByCity(engineDirrect);
            Console.WriteLine("Test_GeobaseEngineDirrect_FindLocationsByCity search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }

        [TestMethod]
        public void Test_GeobaseEngineDirrect_Test_FindLocationByIp()
        {
            var stopwatch = Stopwatch.StartNew();
            GeobaseEngineTest.Test_FindLocationByIp(engineDirrect);
            Console.WriteLine("Test_GeobaseEngineDirrect_Test_FindLocationByIp search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }



        [TestMethod]
        public void Test_GeobaseEngineDirrect_FindLocationsByCity_Multithreading()
        {
            var stopwatch = Stopwatch.StartNew();
            int tasksCount = 1000;

            var tasks = new Task[tasksCount];
            for (int i = 0; i < tasksCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => GeobaseEngineTest.Test_FindLocationsByCity(engineDirrect));
            }
            Task.WaitAll(tasks);

            Console.WriteLine("Test_GeobaseEngineDirrect_FindLocationsByCity_Multithreading search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }

        [TestMethod]
        public void Test_GeobaseEngineDirrect_Test_FindLocationByIp_Multithreading()
        {
            var stopwatch = Stopwatch.StartNew();
            int tasksCount = 1000;

            var tasks = new Task[tasksCount];
            for (int i = 0; i < tasksCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => GeobaseEngineTest.Test_FindLocationByIp(engineDirrect));
            }
            Task.WaitAll(tasks);

            Console.WriteLine("Test_GeobaseEngineDirrect_Test_FindLocationByIp_Multithreading search time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}
