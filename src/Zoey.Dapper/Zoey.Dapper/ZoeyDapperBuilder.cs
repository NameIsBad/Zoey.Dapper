using Microsoft.Extensions.DependencyInjection;
using System;

namespace Zoey.Dapper
{
    public class ZoeyDapperBuilder : IZoeyDapperBuilder
    {
        /// <summary>
        /// 初始化<see cref="ZoeyDapperBuilder"/> 实例.
        /// </summary>
        public ZoeyDapperBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }
    }
}
