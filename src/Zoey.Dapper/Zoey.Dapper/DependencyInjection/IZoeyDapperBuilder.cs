using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IZoeyDapperBuilder
    {
        /// <summary>
        /// 配置ZoeyDapper的<see cref="IServiceCollection"/>
        /// </summary>
        IServiceCollection Services { get; }
    }
}
