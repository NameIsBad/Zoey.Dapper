using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zoey.Dapper.DependencyInjection
{
    public interface IZoeyDapperBuilder
    {
        /// <summary>
        /// 配置ZoeyDapper的<see cref="IServiceCollection"/>
        /// </summary>
        IServiceCollection Services { get; }
    }
}
