using System;
using System.Collections.Generic;
using System.Text;
using Zoey.Dapper.Abstractions;
using Zoey.Dapper.Abstractions.Configuration;

namespace Zoey.Dapper
{
    public class DBDomain : IDBDomain
    {
        public DBDomain()
        {

        }

        public DomainElement GetDomainElement(SqlElement sqlElement)
        {
            return null;
        }
    }
}
