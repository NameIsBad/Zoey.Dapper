using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zoey.Dapper.Configuration
{
    [XmlRoot("sql", Namespace = "http://schema.zoey.com/sql")]
    public class SqlCommandsElement
    {
        [XmlAttribute("domain")]
        public string Domain { get; set; }

        [XmlElement("sql-command")]
        public List<SqlCommandElement> SqlCommands { get; set; }

        [XmlElement("sql-query")]
        public List<SqlQueryElement> SqlQuerys { get; set; }
    }
}
