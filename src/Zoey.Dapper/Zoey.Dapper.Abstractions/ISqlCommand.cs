using System;
using System.Collections.Generic;
using System.Data;

namespace Zoey.Dapper.Abstractions
{
    public interface ISqlCommand
    {
        //DbDataReader ExecuteDataReader(dynamic parameter = null);
        //DataRow ExecuteDataRow(dynamic parameter = null);
        //DataSet ExecuteDataSet(dynamic parameter = null);
        //DataTable ExecuteDataTable(dynamic parameter = null);
        //T ExecuteEntity<T>(dynamic parameter = null);
        //List<T> ExecuteEntityList<T>(dynamic parameter = null);

        IEnumerable<T> Query<T>(dynamic parameter = null, IDbTransaction transaction = null);
        ISqlCommand GetSqlElement(string name);

        IDbConnection GetDbConnection();
    }
}
