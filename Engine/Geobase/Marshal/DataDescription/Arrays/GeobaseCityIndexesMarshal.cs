using System.Runtime.InteropServices;

namespace Engine.Geobase.Marshal.DataDescription
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GeobaseCityIndexesMarshal
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100000)]
        public GeobaseCityIndexMarshal[] CityIndexes;
    }
}