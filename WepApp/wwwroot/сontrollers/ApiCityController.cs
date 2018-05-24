using System.Collections.Generic;
using Engine.Geobase;
using Engine.Geobase.Dirrect;
using Microsoft.AspNetCore.Mvc;

namespace WepApp.wwwroot.Controllers
{
    [Produces("application/json")]
    [Route("city")]
    public class ApiCityController : Controller
    {
        private readonly IGeobaseEngine _geobaseEngine;

        public ApiCityController(IGeobaseEngine geobaseEngine)
        {
            _geobaseEngine = geobaseEngine;
        }

        [Route("locations")]
        public IEnumerable<GeobaseLocationView> Get(string city)
        {
            return _geobaseEngine.FindLocationsByCity(city);
        }
    }
}