using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using Zoey.Dapper;
using Zoey.Dapper.Abstractions;
using Zoey.Dapper.Configuration.MemoryCache;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ZoeyDapperServiceCollectionExtensions
    {
        /// <summary>
        /// 添加到<see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IZoeyDapperBuilder AddZoeyDapperCore(this IServiceCollection services, Action<ZoeyDapperOptions> configureOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            services.Configure(configureOptions);
            return services.AddZoeyDapperCore();
        }

        /// <summary>
        /// 添加到<see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static IZoeyDapperBuilder AddZoeyDapperCore(this IServiceCollection services, string dic)
        {
            if (string.IsNullOrEmpty(dic))
            {
                throw new DirectoryNotFoundException("SQL文件目录不能为空");
            }
            return services.AddZoeyDapperCore(options =>
            {
                options.Path = new List<string>() { dic };
            });
        }

        /// <summary>
        /// 添加到<see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static IZoeyDapperBuilder AddZoeyDapperCore(this IServiceCollection services, List<string> dic)
        {
            if (dic == null || dic.Count == 0)
            {
                throw new DirectoryNotFoundException("SQL文件目录不能为空");
            }
            return services.AddZoeyDapperCore(options =>
            {
                options.Path = dic;
            });
        }

        /// <summary>
        /// 添加到<see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        private static IZoeyDapperBuilder AddZoeyDapperCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.TryAddSingleton<ISqlContext, SqlContext>();
            services.TryAddSingleton<IDBDomain, DBDomain>();
            services.TryAddScoped<ISqlCommand, SqlCommand>();
            services.TryAddSingleton<ISqlPopulate, SqlPopulate>();
            services.TryAddSingleton<ISqlCache, SqlCache>();
            var builder = new ZoeyDapperBuilder(services);
            return builder;
        }
    }
}
