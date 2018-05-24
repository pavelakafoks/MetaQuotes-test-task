using System.Globalization;
using System.Linq;
using Engine.Geobase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    public class GeobaseEngineTest
    {
        public static void Test_FindLocationsByCity(IGeobaseEngine engine)
        {
            var resultsCity_cit_NotFound = engine.FindLocationsByCity("cit_NotFound ").ToList();

            Assert.IsNotNull(resultsCity_cit_NotFound);
            Assert.AreEqual(resultsCity_cit_NotFound.Count, 0);

            var resultsCity_cit_A = engine.FindLocationsByCity("cit_A ").ToList();

            Assert.IsNotNull(resultsCity_cit_A);
            Assert.AreEqual(resultsCity_cit_A.Count, 159);
            Assert.AreEqual(resultsCity_cit_A[0].city, "cit_A ");

            resultsCity_cit_A = engine.FindLocationsByCity("cit_A ").ToList();

            Assert.IsNotNull(resultsCity_cit_A);
            Assert.AreEqual(resultsCity_cit_A.Count, 159);

            var resultsCity_cit_Itovyw_Nural_Su = engine.FindLocationsByCity("cit_Itovyw Nural Su").ToList();

            Assert.IsNotNull(resultsCity_cit_Itovyw_Nural_Su);
            Assert.AreEqual(resultsCity_cit_Itovyw_Nural_Su.Count, 13);

            var resultsCity_cit_I_Of_Wo_Aq_Jofef = engine.FindLocationsByCity("cit_I Of Wo Aq Jofef").ToList();

            Assert.IsNotNull(resultsCity_cit_I_Of_Wo_Aq_Jofef);
            Assert.AreEqual(resultsCity_cit_I_Of_Wo_Aq_Jofef.Count, 9);

            var resultsCity_cit_Y = engine.FindLocationsByCity("cit_Y ").ToList();

            Assert.IsNotNull(resultsCity_cit_Y);
            Assert.AreEqual(resultsCity_cit_Y.Count, 131);

            var resultsCity_cit_YzyrinorijyqotokNotFound = engine.FindLocationsByCity("cit_Yzyrinorijyqotok ").ToList();

            Assert.IsNotNull(resultsCity_cit_YzyrinorijyqotokNotFound);
            Assert.AreEqual(resultsCity_cit_YzyrinorijyqotokNotFound.Count, 0);

            var resultsCity_cit_Yzyrinorijyqotok = engine.FindLocationsByCity("cit_Yzyrinorijyqotok").ToList();

            Assert.IsNotNull(resultsCity_cit_Yzyrinorijyqotok);
            Assert.AreEqual(resultsCity_cit_Yzyrinorijyqotok.Count, 13);
        }

        public static void Test_FindLocationByIp(IGeobaseEngine engine)
        {
            var location = engine.FindLocationByIp("0.0.0.8");

            //Assert.IsNotNull(location);
            //Assert.AreEqual(location.country, "cou_UJO");
            //Assert.AreEqual(location.city, "cit_Elu");
            //Assert.AreEqual(location.organization, "org_Eba Abacir L");
            //Assert.AreEqual(location.latitude.ToString(CultureInfo.InvariantCulture), "-96.2552");
            //Assert.AreEqual(location.longitude.ToString(CultureInfo.InvariantCulture), "-51.5246");
            //Assert.AreEqual(location.postal, "pos_582423");
            //Assert.AreEqual(location.region, "reg_U");

            //location = engine.FindLocationByIp("0.0.0.3");
            //Assert.IsNull(location);

            //location = engine.FindLocationByIp("231.176.8.210");
            //Assert.IsNull(location);

            location = engine.FindLocationByIp("231.176.8.209");
            Assert.AreEqual(location.country, "cou_ORU");
            Assert.AreEqual(location.city, "cit_Yrehecy");
            Assert.AreEqual(location.organization, "org_A Uz Fap");

            location = engine.FindLocationByIp("138.89.38.175");
            Assert.AreEqual(location.country, "cou_ORO");
            Assert.AreEqual(location.city, "cit_Omoromew Gilolo");
            Assert.AreEqual(location.organization, "org_Ir Setuke");

            location = engine.FindLocationByIp("138.89.39.112");
            Assert.AreEqual(location.country, "cou_AF");
            Assert.AreEqual(location.city, "cit_Ix ");
            Assert.AreEqual(location.organization, "org_Emitepegazijedav C Fuga");
            Assert.AreEqual(location.latitude.ToString(CultureInfo.InvariantCulture), "170.1512");
            Assert.AreEqual(location.longitude.ToString(CultureInfo.InvariantCulture), "98.5677");
            Assert.AreEqual(location.postal, "pos_1754");
            Assert.AreEqual(location.region, "reg_Igin");
        }
    }
}