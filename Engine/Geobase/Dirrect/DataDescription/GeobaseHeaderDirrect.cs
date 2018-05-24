using Engine.Attribute;

namespace Engine.Geobase.Dirrect.DataDescription
{
    [LengthBytes(60)]
    public class GeobaseHeaderDirrect
    {
        [LengthBytes(4)]
        public int Version { get; set; }

        [LengthBytes(32)]
        public string Name { get; set; }

        [LengthBytes(8)]
        public ulong Timestamp { get; set; }

        [LengthBytes(4)]
        public int Records { get; set; }

        [LengthBytes(4)]
        public uint OffsetRanges { get; set; }

        [LengthBytes(4)]
        public uint OffsetCities { get; set; }

        [LengthBytes(4)]
        public uint OffsetLocations { get; set; }
    }
}
