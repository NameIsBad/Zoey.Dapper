using System;
using System.Collections.Generic;
using System.Text;

namespace Zoey.Dapper.Abstractions.Configuration
{

    /// <summary>
    /// 执行操作
    /// </summary>
    public class SqlCommandElement : SqlElement
    {
        public SqlCommandElement()
            : base()
        {
        }

        protected override SqlElement CreateInstance()
        {
            return new SqlCommandElement();
        }
    }
}
