using Zoey.Dapper.Configuration;

namespace Zoey.Dapper.Abstractions
{
    public interface ISqlContext
    {
        SqlElement GetSqlElement(string name);
    }
}
