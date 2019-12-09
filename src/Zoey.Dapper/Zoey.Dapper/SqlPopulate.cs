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
    public class SqlPopulate : ISqlPopulate
    {
        private readonly ZoeyDapperOptions _options;
        private readonly IFileProvider _fileProvider;

        public SqlPopulate(IFileProvider fileProvider, IOptions<ZoeyDapperOptions> options)
        {
            _fileProvider = fileProvider;
            _options = options.Value;
        }

        public IDictionary<string, SqlElement> GetAllSql()
        {
            var result = PopulateSql();
            return result;
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


    }
}
