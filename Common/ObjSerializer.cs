using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ondato.Common
{
    public static class ObjSerializer
    {
        [ThreadStatic]
        private static readonly BinaryFormatter binaryFormatter = new();

        public static byte[] Serialize(object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                binaryFormatter.Serialize(memoryStream, obj);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                return memoryStream.ToArray();
            }
        }

        public static object Deserialize(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                return binaryFormatter.Deserialize(memoryStream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
        }
    }
}
