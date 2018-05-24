using System.Runtime.InteropServices;

namespace Engine.Geobase.Marshal.DataDescription
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GeobaseIpRangesMarshal
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100000)]
        public GeobaseIpRangeMarshal[] IpRanges;
    }
}
