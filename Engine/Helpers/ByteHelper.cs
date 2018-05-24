using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Engine.Geobase.Dirrect;

namespace Engine.Helpers
{
    public static class ByteHelper
    {
        public static GeobaseLocationView GetLocation(byte[] bytes, int offset)
        {
            var toRet = new GeobaseLocationView
            {
                country = GetString(bytes, 8, ref offset, true),
                region = GetString(bytes, 12, ref offset, true),
                postal = GetString(bytes, 12, ref offset, true),
                city = GetString(bytes, 24, ref offset, true),
                organization = GetString(bytes, 32, ref offset, true),
                latitude = (decimal)GetFloat(bytes, ref offset, true),
                longitude = (decimal)GetFloat(bytes, ref offset, true)
            };
            return toRet;
        }

        public static T BytesToStruct<T> (byte[] by)
        {
            var handle = GCHandle.Alloc(by, GCHandleType.Pinned);
            T temp = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return temp;
        }

        public static int GetInt(byte[] bytes, ref int offset, bool moveOffset)
        {
            if (moveOffset)
            {
                return bytes[offset++]
                       | (bytes[offset++] << 8)
                       | (bytes[offset++] << 16)
                       | (bytes[offset++] << 24);
            }
            else
            {
                return bytes[offset]
                       | (bytes[offset + 1] << 8)
                       | (bytes[offset + 2] << 16)
                       | (bytes[offset + 3] << 24);

            }
        }

        public static string GetString(byte[] bytes, int length, ref int offset, bool moveOffset)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                var position = offset + i;
                if (bytes[position] != '\0')
                {
                    sb.Append((char)bytes[position]);
                }
            }

            if (moveOffset)
            {
                offset += length;
            }
            return sb.ToString();

            // just for information - works slower:

            //var toRet = Encoding.ASCII.GetString(bytes, offset, length);
            //int i;
            //for (i = 0; i < toRet.Length; i++)
            //{
            //    if (toRet[i] == '\0') break;
            //}
            //if (offset != toRet.Length - 1)
            //{
            //    toRet = toRet.Substring(0, i);
            //}
            //offset += length;
            //return toRet;
        }

        public static ulong GetULong(byte[] bytes, ref int offset, bool moveOffset)
        {
            var toRet = BitConverter.ToUInt64(bytes, offset);
            if (moveOffset)
            {
                offset += sizeof(ulong);
            }
            return toRet;
        }

        public static uint GetUInt(byte[] bytes, ref int offset, bool moveOffset)
        {
            var toRet = BitConverter.ToUInt32(bytes, offset);

            // Code below not faster (unsafe)
            //uint toRet;
            //fixed (byte* p = bytes)
            //{
            //    toRet = *((uint*)(p + index));
            //}

            if (moveOffset)
            {
                offset += sizeof(uint);
            }

            return toRet;
        }

        public static float GetFloat(byte[] bytes, ref int offset, bool moveOffset)
        {
            var toRet = BitConverter.ToSingle(bytes, offset);
            if (moveOffset)
            {
                offset += sizeof(float);
            }
            return toRet;
        }

        // Attention - next procedure (CompareStringsAsBytes) slower than CompareStrings !
        // I just looked for the best solution

        // output equal to string.CompareOrdinal
        // string2 can contain /0 characters
        // Less - than zero strA is less than strB.
        // Zero - strA and strB are equal.
        // Greater than zero - strA is greater than strB. 
        public static int CompareStringsAsBytes(byte[] strA, ArraySegment<byte> strB)
        {
            if (strA.Length > strB.Count)
            {
                return 1;
            }

            for (int i = 0; i < strA.Length; i++)
            {
                if (strB.Array[strB.Offset + i] == '\0')
                {
                    if (i == strA.Length)
                    {
                        return 0;
                    }

                    return 1;
                }

                if (strA[i] > strB.Array[strB.Offset + i])
                {
                    return 1;
                }
                if (strA[i] < strB.Array[strB.Offset + i])
                {
                    return -1;
                }
            }

            if (strB.Count > strA.Length && strB.Array[strB.Offset + strA.Length] != '\0')
            {
                return -1;
            }

            return 0;
        }

        public static int CompareStrings(string strA, byte[] bytes, int length, int offset)
        {
            var strB = GetString(bytes, length, ref offset, false);
            return string.CompareOrdinal(strA, strB);
        }

        public static byte[] GetBytes(byte[] bytes, int offset, int count)
        {
            //var toRet = new byte[count];
            //Array.Copy(bytes, offset, toRet, 0, count);
            //return toRet;

            return new ArraySegment<byte>(bytes, offset, count).ToArray();
        }
    }
}