using System.Runtime.InteropServices;

namespace Engine.Geobase.Combined.DataDescription
{
    [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi, Pack = 1)]
    public struct GeobaseLocationCityCombined
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string City;
    }
}