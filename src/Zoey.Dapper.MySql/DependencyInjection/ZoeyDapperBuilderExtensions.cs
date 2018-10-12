using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using Zoey.Dapper.Abstractions;

namespace Zoey.Dapper.DependencyInjection
{
    public static class ZoeyDapperBuilderExtensions
    {
        public static IZoeyDapperBuilder AddMSSQLserver(this IZoeyDapperBuilder builder, Action<ZoeyDapperDataBaseOptions> configureOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            builder.Services.AddScoped<IDbConnection, MySqlConnection>();
            builder.Services.Configure(configureOptions);
            return builder;
        }
    }
}
