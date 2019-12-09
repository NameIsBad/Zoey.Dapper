using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Zoey.Dapper.Abstractions;
using Zoey.Dapper.Configuration;

namespace Zoey.Dapper
{
    public class SqlContext : ISqlContext
    {
        private readonly ISqlCache _sqlCache;
        private readonly ZoeyDapperOptions _options;
        private readonly IFileProvider _fileProvider;

        public SqlContext(ISqlCache sqlCache, IFileProvider fileProvider, IOptions<ZoeyDapperOptions> options)
        {
            _sqlCache = sqlCache;
            _fileProvider = fileProvider;
            _options = options.Value;
            WatchAndCheckSqlDir();
        }

        public SqlElement GetSqlElement(string name)
        {
            var config = _sqlCache.GetElementByName(name);
            return config;
        }


        /// <summary>
        /// 监视和检查Sql文件夹
        /// </summary>
        private void WatchAndCheckSqlDir()
        {
            foreach (var subpath in _options.Path)
            {
                var dir = _fileProvider.GetDirectoryContents(subpath);
                if (!dir.Exists)
                    throw new DirectoryNotFoundException($"SQL文件目录不存在：{subpath}");
                var watchFileFilter = $"{subpath}/**/{_options.WatchFileFilter ?? "*.*"}";
                Action<object> callback = null;
                callback = _ =>
                {
                    _sqlCache.Clear();
                    _fileProvider.Watch(watchFileFilter).RegisterChangeCallback(callback, null);
                };
                _fileProvider.Watch(watchFileFilter).RegisterChangeCallback(callback, null);
            }
            _sqlCache.Set();
        }

    }
}