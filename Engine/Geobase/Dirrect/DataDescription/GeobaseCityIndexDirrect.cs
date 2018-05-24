using Engine.Attribute;

namespace Engine.Geobase.Dirrect
{
    [LengthBytes(4)]
    public class GeobaseCityIndexDirrect
    {
        [LengthBytes(4)]
        public uint LocationOffset { get; set; }
    }
}
