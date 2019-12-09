using System;
using System.Collections.Generic;
using System.Text;
using Zoey.Dapper.Configuration;

namespace Zoey.Dapper.Abstractions
{
    public interface ISqlPopulate
    {
        IDictionary<string, SqlElement> GetAllSql();
    }
}
