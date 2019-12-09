using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Data.SqlClient;
using Zoey.Dapper.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
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
            builder.Services.AddScoped<IDbConnection, SqlConnection>();
            builder.Services.Configure(configureOptions);
            return builder;
        }
    }
}
