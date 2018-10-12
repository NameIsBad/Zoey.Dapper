using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Zoey.Dapper.Abstractions;
using Zoey.Dapper.Abstractions.Configuration;

namespace Zoey.Dapper
{
    public class SqlCommand : ISqlCommand
    {
        protected SqlElement sqlElement;
        private readonly ZoeyDapperDataBaseOptions _options;
        private readonly IDbConnection _dbConnection;
        private readonly ISqlContext _sqlContext;

        public SqlCommand(IDbConnection dbConnection, ISqlContext sqlContext, IOptions<ZoeyDapperDataBaseOptions> options)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _sqlContext = sqlContext;
        }

        public ISqlCommand GetSqlElement(string name)
        {
            sqlElement = _sqlContext.GetSqlElement(name);
            return this;
        }

        public IEnumerable<T> Query<T>(dynamic parameter = null, IDbTransaction transaction = null)
        {
            GetDbConnection();
            try
            {
                return _dbConnection.Query<T>(sqlElement.CommandText, (object)parameter, transaction, true, sqlElement.TimeOut, sqlElement.CommandType);
            }
            catch (Exception ex)
            {
                throw new ZoeyDataAccessException(ex, _dbConnection != null ? _dbConnection.ConnectionString : sqlElement.Domain, sqlElement.CommandText, parameter);
            }
            finally
            {
                if (transaction == null)
                    _dbConnection?.Dispose();
            }
        }


        private void EnsureConnectionIsOpen()
        {
            var isClosed = _dbConnection.State == ConnectionState.Closed;
            if (isClosed)
                this._dbConnection.Open();
        }

        public IDbConnection GetDbConnection()
        {
            try
            {
                DomainElement domain = string.IsNullOrEmpty(sqlElement.Domain)
                ? _options.DomainElements.First()
                : _options.DomainElements.First(p => p.Name == sqlElement.Domain);
                var databaseName = string.Empty;
                if (sqlElement is SqlCommandElement)
                {
                    databaseName = domain.MasterSlaves.Master.First();
                }
                else if (sqlElement is SqlQueryElement)
                {
                    // TODO: loadbalance
                    databaseName = domain.MasterSlaves.Slaves.First();
                }
                var connectionInfo = _options.DatabaseElements.First(t => t.Name == databaseName);
                _dbConnection.ConnectionString = connectionInfo.ConnectionString;
                EnsureConnectionIsOpen();
            }
            catch (Exception ex)
            {
                _dbConnection?.Close();
                throw ex;
            }
            return _dbConnection;
        }
    }
}
