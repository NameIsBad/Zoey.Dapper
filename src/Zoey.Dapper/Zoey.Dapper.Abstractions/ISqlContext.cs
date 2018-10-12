using Zoey.Dapper.Abstractions.Configuration;

namespace Zoey.Dapper.Abstractions
{
    public interface ISqlContext
    {
        SqlElement GetSqlElement(string name);
    }
}
