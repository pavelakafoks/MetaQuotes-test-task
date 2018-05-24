using System.Collections.Generic;
using Engine.Geobase.Dirrect;

namespace Engine.Geobase
{
    public interface IGeobaseEngine
    {
        GeobaseLocationView FindLocationByIp(string ipString);
        IEnumerable<GeobaseLocationView> FindLocationsByCity(string city);
    }
}
