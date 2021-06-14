using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ondato.Common
{
    public static class ObjSerializer
    {
        [ThreadStatic]
        private static BinaryFormatter _binaryFormatter = new BinaryFormatter();
        private static BinaryFormatter GetBinaryFormatter() => _binaryFormatter ??= new BinaryFormatter();

        public static byte[] Serialize(object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                GetBinaryFormatter().Serialize(memoryStream, obj);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                return memoryStream.ToArray();
            }
        }

        public static object Deserialize(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                return GetBinaryFormatter().Deserialize(memoryStream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
        }
    }
}
