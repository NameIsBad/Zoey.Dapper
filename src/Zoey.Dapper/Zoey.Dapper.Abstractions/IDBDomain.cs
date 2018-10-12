using System;
using System.Collections.Generic;
using System.Text;
using Zoey.Dapper.Abstractions.Configuration;

namespace Zoey.Dapper.Abstractions
{
    public interface IDBDomain
    {
        DomainElement GetDomainElement(SqlElement sqlElement);
    }
}
