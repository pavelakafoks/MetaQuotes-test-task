using Engine.Attribute;

namespace Engine.Geobase.Dirrect.DataDescription
{
    [LengthBytes(96)]
    public class GeobaseLocationDirrect
    {
        [LengthBytes(8)]
        public string Country { get; set; }

        [LengthBytes(12)]
        public string Region { get; set; }

        [LengthBytes(12)]
        public string Postal { get; set; }

        [LengthBytes(24)]
        public string City { get; set; }

        [LengthBytes(32)]
        public string Organization { get; set; }

        [LengthBytes(4)]
        public float Latitude { get; set; }

        [LengthBytes(4)]
        public float Longitude { get; set; }
    }
}