using System.Runtime.InteropServices;

namespace Engine.Geobase.Marshal.DataDescription
{
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct GeobaseIpRangeMarshal
    {
        [FieldOffset(0)]
        public uint IpFrom;
        [FieldOffset(4)]
        public uint IpTo;
        [FieldOffset(8)]
        public uint LocationIndex;
    }
}