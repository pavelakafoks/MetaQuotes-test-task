using System.Runtime.InteropServices;

namespace Engine.Geobase.Marshal.DataDescription
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GeobaseDataMarshal
    {
        public GeobaseHeaderMarshal Header;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100000)]
        public GeobaseIpRangeMarshal[] IpRanges;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100000)]
        public GeobaseLocationMarshal[] Locations;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100000)]
        public GeobaseCityIndexMarshal[] CityIndexes;
    }
}
