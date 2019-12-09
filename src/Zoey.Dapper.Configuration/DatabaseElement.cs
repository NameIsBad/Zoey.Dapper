namespace Zoey.Dapper.Configuration
{
    public class DatabaseElement
    {
        public DatabaseElement(string name, string connectionString)
            : this(name, connectionString, string.Empty)
        {

        }

        public DatabaseElement(string name, string connectionString, string provider)
        {
            Name = name;
            ConnectionString = connectionString;
            Provider = provider;
        }

        public string ConnectionString { get; set; }

        public string Name { get; set; }

        public string Provider { get; set; }
    }
}
