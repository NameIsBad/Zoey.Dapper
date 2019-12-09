using Zoey.Dapper.Configuration;

namespace Zoey.Dapper.Abstractions
{
    public interface IDBDomain
    {
        DomainElement GetDomainElement(SqlElement sqlElement);
    }
}
