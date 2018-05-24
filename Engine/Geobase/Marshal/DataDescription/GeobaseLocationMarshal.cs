using System.Runtime.InteropServices;

namespace Engine.Geobase.Marshal.DataDescription
{
    [StructLayout(LayoutKind.Sequential, Size = 96, CharSet = CharSet.Ansi, Pack = 1)]
    public struct GeobaseLocationMarshal
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Country;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Region;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Postal;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string City;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Organization;
        public float Latitude;
        public float Longitude;
    }
}