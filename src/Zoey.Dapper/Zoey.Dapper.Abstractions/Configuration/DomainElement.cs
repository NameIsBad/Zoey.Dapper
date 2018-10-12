using System.Collections.Generic;
using System.Linq;

namespace Zoey.Dapper
{
    public class DomainElement
    {
        public MasterSlaves MasterSlaves { get; set; }

        public string Name { get; set; }
    }

    public class MasterSlaves
    {
        public MasterSlaves()
        {
            this.Master = new List<string>();
            this.Slaves = new List<string>();
        }

        public MasterSlaves(List<string> master, List<string> slave)
        {
            this.Master = master;
            this.Slaves = slave;
        }
        public MasterSlaves(string master, string slave)
        {
            this.Master = new List<string>() { master };
            this.Slaves = new List<string>() { slave };
        }

        public void AddMaster(string database)
        {
            Master.Add(database);
        }

        public void AddSlave(string database)
        {
            this.Slaves.Add(database);
        }
        
        public List<string> Master { get; set; }
        
        public List<string> Slaves { get; set; }
    }
}
