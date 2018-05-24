using Engine.Attribute;

namespace Engine.Geobase.Dirrect.DataDescription
{
    [LengthBytes(12)]
    public class GeobaseIpRangeDirrect
    {
        [LengthBytes(4)]
        public uint IpFrom { get; set; }

        [LengthBytes(4)]
        public uint IpTo { get; set; }

        [LengthBytes(4)]
        public uint LocationIndex { get; set; }
    }
}
