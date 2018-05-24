using System.Runtime.InteropServices;
using Engine.Geobase.Marshal.DataDescription;

namespace Engine.Geobase.Combined.DataDescription
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GeobaseLocationCityIndexesCombined
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100000)]
        public GeobaseCityIndexMarshal[] CityIndexes;
    }
}