using Engine.Geobase;
using Engine.Geobase.Dirrect;
using Microsoft.AspNetCore.Mvc;

namespace WepApp.wwwroot.Controllers
{
    [Produces("application/json")]
    [Route("ip")]
    public class ApiIpController : Controller
    {
        private readonly IGeobaseEngine _geobaseEngine;

        public ApiIpController(IGeobaseEngine geobaseEngine)
        {
            _geobaseEngine = geobaseEngine;
        }

        [Route("location")]
        public GeobaseLocationView Get(string ip)
        {
            return _geobaseEngine.FindLocationByIp(ip);
        }
    }
}