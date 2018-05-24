using System.Runtime.InteropServices;

namespace Engine.Geobase.Marshal.DataDescription
{
    [StructLayout(LayoutKind.Sequential, Size = 60, CharSet = CharSet.Ansi, Pack = 1)]
    public struct GeobaseHeaderMarshal
    {
        public int Version;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string NameLast;
        public ulong TimeStamp;
        public int Records;
        public uint OffsetRanges;
        public uint OffsetCities;
        public uint OffsetLocations;
    }
}