//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.FileProviders;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.IO;
//using Zoey.Dapper.Abstractions;

//namespace Zoey.Dapper.Configuration.MemoryCache
//{
//    public class SqlContext : ISqlContext
//    {
//        private const string CacheKey = "Zoey_Dapper_Element_Key";
//        private readonly IFileProvider _fileProvider;
//        private readonly ZoeyDapperOptions _options;
//        private readonly IMemoryCache _memoryCache;

//        public SqlContext(IFileProvider fileProvider, IOptions<ZoeyDapperOptions> options, IMemoryCache memoryCache)
//        {
//            _memoryCache = memoryCache;
//            _fileProvider = fileProvider;
//            _options = options.Value;
//            WatchAndCheckSqlDir();
//        }

//        public SqlElement GetSqlElement(string name)
//        {
//            var allSqlElement = _memoryCache.GetOrCreate(CacheKey, t =>
//            {
//                var dic = new ConcurrentDictionary<string, SqlElement>();
//                var sql = LoadSql();
//                foreach (var command in sql)
//                {
//                    if (dic.ContainsKey(command.Name))
//                        throw new ArgumentException($"已添加了具有相同键的项.当前键:{command.Name}");
//                    dic.GetOrAdd(command.Name, command);
//                }
//                return dic;
//            });

//            if (!allSqlElement.ContainsKey(name))
//                throw new KeyNotFoundException($"未发现名为【{name}】的SQL数据");
//            var config = allSqlElement[name];
//            return config;
//        }

//        private IEnumerable<SqlElement> LoadSql()
//        {
//            var element = new List<SqlElement>();
//            foreach (var subpath in _options.Path)
//            {
//                RecursionLoadSqlByDir(subpath, element);
//            }
//            return element;
//        }

//        /// <summary>
//        /// 递归查询
//        /// </summary>
//        /// <param name="subpath"></param>
//        /// <param name="sqlElements"></param>
//        private void RecursionLoadSqlByDir(string subpath, List<SqlElement> sqlElements)
//        {
//            var dir = _fileProvider.GetDirectoryContents(subpath);
//            foreach (var file in dir)
//            {
//                if (!file.IsDirectory)
//                {
//                    var cmds = XmlSerialization.Deserialize<SqlCommandsElement>(file.PhysicalPath);
//                    if (string.IsNullOrEmpty(cmds.Domain))
//                        throw new InvalidOperationException($"文件:{file.PhysicalPath}.缺少【domain】属性.");

//                    foreach (var sqlQuery in cmds.SqlQuerys)
//                    {
//                        if (string.IsNullOrEmpty(sqlQuery.Domain))
//                            sqlQuery.Domain = cmds.Domain;
//                        sqlElements.Add(sqlQuery);
//                    }

//                    foreach (var sqlCommand in cmds.SqlCommands)
//                    {
//                        if (string.IsNullOrEmpty(sqlCommand.Domain))
//                            sqlCommand.Domain = cmds.Domain;
//                        sqlElements.Add(sqlCommand);
//                    }
//                }
//                else
//                    RecursionLoadSqlByDir($"{subpath}/{file.Name}", sqlElements);
//            }
//        }

//        /// <summary>
//        /// 监视和检查Sql文件夹
//        /// </summary>
//        private void WatchAndCheckSqlDir()
//        {
//            foreach (var subpath in _options.Path)
//            {
//                var dir = _fileProvider.GetDirectoryContents(subpath);
//                if (!dir.Exists)
//                    throw new DirectoryNotFoundException($"SQL文件目录不存在：{subpath}");
//                var watchFileFilter = $"{subpath}/**/{_options.WatchFileFilter ?? "*.*"}";
//                Action<object> callback = null;
//                callback = _ =>
//                {
//                    _memoryCache.Remove(CacheKey);
//                    _fileProvider.Watch(watchFileFilter).RegisterChangeCallback(callback, null);
//                };
//                _fileProvider.Watch(watchFileFilter).RegisterChangeCallback(callback, null);
//            }
//        }
//    }
//}
