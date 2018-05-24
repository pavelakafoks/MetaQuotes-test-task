using System.Runtime.InteropServices;

namespace Engine.Geobase.Marshal.DataDescription
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GeobaseLocationsMarshal
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2000)]
        public GeobaseLocationMarshal[] Locations;
    }
}
