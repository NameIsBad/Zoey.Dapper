using System;
using System.Collections.Generic;
using Zoey.Dapper.Configuration;

namespace Zoey.Dapper.Abstractions
{
    public interface ISqlCache
    {
        void Set();
        SqlElement GetElementByName(string name);
        void Clear();
    }
}
