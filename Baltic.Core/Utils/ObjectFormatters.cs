using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Baltic.Core.Utils
{
    public static class ObjectFormatters
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            if (null == obj)
            {
                return default;
            }

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        public static object ObjectFromByteArray(byte[] data)
        {
            if (null == data)
            {
                return default;
            }

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(data))
            {
                return binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}