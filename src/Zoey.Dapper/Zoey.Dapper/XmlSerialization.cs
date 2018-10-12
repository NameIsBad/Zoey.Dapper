using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Zoey.Dapper
{
    public static class XmlSerialization
    {
        private static ConcurrentDictionary<Type, XmlSerializer> cache;

        static XmlSerialization()
        {
            cache = new ConcurrentDictionary<Type, XmlSerializer>();
        }

        public static T Deserialize<T>(string uri)
        {
            var content = GetFileResource(uri);
            using (var ms = new MemoryStream(content))
            {
                var result = GetSerializer<T>().Deserialize(ms);
                return result == null ? default(T) : (T)result;
            }
        }
        private static byte[] GetFileResource(string uri)
        {
            if (!File.Exists(uri))
                throw new FileNotFoundException(uri);
            byte[] returnValue;
            using (FileStream s = File.Open(uri, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                returnValue = new byte[s.Length];
                s.Read(returnValue, 0, (int)s.Length);
            }
            return returnValue;
        }

        private static XmlSerializer GetSerializer<T>()
        {
            var type = typeof(T);
            return cache.GetOrAdd(type, XmlSerializer.FromTypes(new[] { type }).FirstOrDefault());
        }
    }
}