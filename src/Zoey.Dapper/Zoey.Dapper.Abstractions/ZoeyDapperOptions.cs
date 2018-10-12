using System;
using System.Collections.Generic;
using System.Text;

namespace Zoey.Dapper.Abstractions
{
    public class ZoeyDapperOptions
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public IEnumerable<string> Path { get; set; }

        /// <summary>
        /// 监听文件类型
        /// 默认:*.xml
        /// </summary>
        public string WatchFileFilter { get; set; } = "*.xml";

        /// <summary>
        /// 启动读写分离
        /// </summary>
        public bool StartProxy { get; set; } = false;
    }
}
