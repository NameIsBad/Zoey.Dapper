﻿using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Zoey.Dapper.Abstractions;
using Zoey.Dapper.Abstractions.Configuration;

namespace Zoey.Dapper
{
    public class SqlContext : ISqlContext
    {
        private readonly IFileProvider _fileProvider;
        private readonly ZoeyDapperOptions _options;
        private ConcurrentDictionary<string, SqlElement> _sqlElement;
        private static readonly object _lock = new object();

        public SqlContext(IFileProvider fileProvider,IOptions<ZoeyDapperOptions> options)
        {
            _fileProvider = fileProvider;
            _options = options.Value;
            WatchAndCheckSqlDir();
        }
        
        public SqlElement GetSqlElement(string name)
        {
            if (!this.Commands.ContainsKey(name))
                throw new KeyNotFoundException($"未发现名为【{name}】的SQL数据");
            var config = this.Commands[name];
            return config;
        }


        public ConcurrentDictionary<string, SqlElement> Commands
        {
            get
            {
                if (_sqlElement == null)
                    this._sqlElement = this.PopulateSql();
                return _sqlElement;
            }
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
                    Clear();
                    _fileProvider.Watch(watchFileFilter).RegisterChangeCallback(callback, null);
                };
                _fileProvider.Watch(watchFileFilter).RegisterChangeCallback(callback, null);
            }
        }

        private ConcurrentDictionary<string, SqlElement> PopulateSql()
        {
            var dic = new ConcurrentDictionary<string, SqlElement>();
            var sql = LoadSql();
            foreach (var command in sql)
            {
                if (dic.ContainsKey(command.Name))
                    throw new ArgumentException($"已添加了具有相同键的项.当前键:{command.Name}");
                dic.GetOrAdd(command.Name, command);
            }
            return dic;
        }

        private IEnumerable<SqlElement> LoadSql()
        {
            var element = new List<SqlElement>();
            foreach (var subpath in _options.Path)
            {
                RecursionLoadSqlByDir(subpath, element);
            }
            return element;
        }

        private void RecursionLoadSqlByDir(string subpath, List<SqlElement> sqlElements)
        {
            var dir = _fileProvider.GetDirectoryContents(subpath);
            foreach (var file in dir)
            {
                if (!file.IsDirectory)
                {
                    var cmds = XmlSerialization.Deserialize<SqlCommandsElement>(file.PhysicalPath);
                    if (string.IsNullOrEmpty(cmds.Domain))
                        throw new InvalidOperationException($"文件:{file.PhysicalPath}.缺少【domain】属性.");

                    foreach (var sqlQuery in cmds.SqlQuerys)
                    {
                        if (string.IsNullOrEmpty(sqlQuery.Domain))
                            sqlQuery.Domain = cmds.Domain;
                        sqlElements.Add(sqlQuery);
                    }

                    foreach (var sqlCommand in cmds.SqlCommands)
                    {
                        if (string.IsNullOrEmpty(sqlCommand.Domain))
                            sqlCommand.Domain = cmds.Domain;
                        sqlElements.Add(sqlCommand);
                    }
                }
                else
                    RecursionLoadSqlByDir($"{subpath}/{file.Name}", sqlElements);
            }
        }

        private void Clear()
        {
            _sqlElement?.Clear();
            _sqlElement = null;
        }
    }
}