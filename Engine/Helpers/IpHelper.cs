using System;
using System.Net;

namespace Engine.Helpers
{
    public static class IpHelper
    {
        public static uint IpStringToUint(string ipString)
        {
            var ipAddress = IPAddress.Parse(ipString);
            var ipBytes = ipAddress.GetAddressBytes();
            var ip = (uint)ipBytes [0] << 24;
            ip += (uint)ipBytes [1] << 16;
            ip += (uint)ipBytes [2] <<8;
            ip += (uint)ipBytes [3];
            return ip;
            
            // return BitConverter.ToUInt32(IPAddress.Parse(ipString).GetAddressBytes(), 0); // reverse order of bytes
        }

        public static string IpUintToString(uint ipUint)
        {
            var ipBytes = BitConverter.GetBytes(ipUint);
            var ipBytesRevert = new byte[4];
            ipBytesRevert[0] = ipBytes[3];
            ipBytesRevert[1] = ipBytes[2];
            ipBytesRevert[2] = ipBytes[1];
            ipBytesRevert[3] = ipBytes[0];
            return new IPAddress(ipBytesRevert).ToString();
            
            // return new IPAddress(BitConverter.GetBytes(ipUint)).ToString(); // reverse order of bytes
        }
    }
}