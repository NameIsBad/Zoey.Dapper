using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using Zoey.Dapper.Abstractions;

namespace Zoey.Dapper.Configuration.MemoryCache
{
    public class SqlCache : ISqlCache
    {
        //private const string CacheKey = "Zoey_Dapper_Element_Key_{0}";        
        private const string CacheKey = "Zoey_Dapper_Element_Key";
        private readonly IMemoryCache _memoryCache;
        private readonly ISqlPopulate _sqlPopulate;

        public SqlCache(IMemoryCache memoryCache, ISqlPopulate sqlPopulate)
        {
            _memoryCache = memoryCache;
            _sqlPopulate = sqlPopulate;
        }

        public SqlElement GetElementByName(string name)
        {
            var allElement = _memoryCache.Get<IDictionary<string, SqlElement>>(CacheKey);
            if (!allElement.ContainsKey(name))
                throw new Exception($"未发现名为【{name}】的SQL数据");
            return allElement[name];

            //var element = _memoryCache.Get<SqlElement>(string.Format(CacheKey, name));
            //if (element == null)
            //    throw new Exception($"未发现名为【{name}】的SQL数据");
            //return element;
        }

        public void Clear()
        {
            _memoryCache.Remove(CacheKey);

            //_memoryCache.Clear();
        }

        public void Set()
        {
            var elements = _sqlPopulate.GetAllSql();
            _memoryCache.Set(CacheKey, elements);

            //foreach (var item in elements)
            //{
            //    _memoryCache.Set(string.Format(CacheKey, item.Key), item.Values);
            //}
        }




    }
}
