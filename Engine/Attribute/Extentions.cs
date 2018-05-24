using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Engine.Attribute
{
    public static class Extentions
    {
        public static int GetBytesLength(Type type)
        {
            if (type.StructLayoutAttribute != null && type.StructLayoutAttribute.Size != 0)
            {
                return type.StructLayoutAttribute.Size;
            }
            var attrs = type.GetCustomAttributes(false);
            foreach (var attrCurrent in attrs)
            {
                if (attrCurrent is LengthBytesAttribute attrLengthBytesAttribute)
                {
                    return attrLengthBytesAttribute.Length;
                }
            }
            return 0;
        }

        public static int GetBytesLength(Type type, string propertyName)
        {
            var prop = type.GetProperties().FirstOrDefault(x => x.Name == propertyName);
            if (prop != null)
            {
                var attrs = prop.GetCustomAttributes(false);
                foreach (var attrCurrent in attrs)
                {
                    if (attrCurrent is LengthBytesAttribute attrLengthBytesAttribute)
                    {
                        return attrLengthBytesAttribute.Length;
                    }
                }
            }
            return 0;
        }

        public static int GetBytesLength(this object obj)
        {
            if (obj == null)
            {
                throw new Exception("Impossible to get an Attribute for NULL object. Use the instance of object.");
            }

            return GetBytesLength(obj.GetType());
        }

        public static int GetBytesLength(this object obj, string propertyName)
        {
            if (obj == null)
            {
                throw new Exception("Impossible to get an Attribute for NULL object. Use the instance of object.");
            }

            return GetBytesLength(obj.GetType(), propertyName);
        }
    }
}
