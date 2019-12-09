using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Zoey.Dapper.Abstractions
{
    /// <summary>
    /// TODO:应该放在？？
    /// </summary>
    public static class XmlSerialization
    {
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
            return XmlSerializer.FromTypes(new[] { type }).FirstOrDefault();
        }
    }
}