using Engine.Geobase.Dirrect;
using Engine.Geobase.Marshal;
using Engine.Geobase.Marshal.DataDescription;

namespace Engine.Helpers
{
    public class TransformationHelper
    {
        public static GeobaseLocationView GetLocationView(GeobaseLocationMarshal location)
        {
            return new GeobaseLocationView()
            {
                city = location.City,
                country = location.Country,
                latitude = (decimal)location.Latitude,
                longitude = (decimal)location.Longitude,
                organization = location.Organization,
                postal = location.Postal,
                region = location.Region
            };
        }
    }
}
