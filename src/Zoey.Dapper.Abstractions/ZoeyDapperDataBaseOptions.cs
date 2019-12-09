using System.Collections.Generic;
using Zoey.Dapper.Configuration;

namespace Zoey.Dapper.Abstractions
{
    public class ZoeyDapperDataBaseOptions
    {
        public ZoeyDapperDataBaseOptions()
        {
            DatabaseElements = new List<DatabaseElement>();
            DomainElements = new List<DomainElement>();
        }
        public IEnumerable<DatabaseElement> DatabaseElements { get; set; }
        public IEnumerable<DomainElement> DomainElements { get; set; }
    }
}
