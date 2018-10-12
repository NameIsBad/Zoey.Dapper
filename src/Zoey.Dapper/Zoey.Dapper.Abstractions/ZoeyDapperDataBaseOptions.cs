using System;
using System.Collections.Generic;
using System.Text;
using Zoey.Dapper.Abstractions.Configuration;

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
