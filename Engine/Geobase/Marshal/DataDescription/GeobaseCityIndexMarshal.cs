using System.Runtime.InteropServices;

namespace Engine.Geobase.Marshal.DataDescription
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct GeobaseCityIndexMarshal
    {
        [FieldOffset(0)]
        public uint LocationOffset;
    }
}
