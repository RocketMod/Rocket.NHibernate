using System;
using System.Linq;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.Core.Configuration;

namespace Rocket.NHibernate
{
    public class NHibernateConnectionDescriptor : INHibernateConnectionDescriptor
    {
        private readonly IRuntime _runtime;
        private readonly IConfiguration _config;
        private ConfigurationContext _context;

        public NHibernateConnectionDescriptor(IRuntime runtime, IConfiguration config)
        {
            _runtime = runtime;
            _config = config;
        }

        public NHibernateConnectionInfo ConnectionInfo
        {
            get
            {
                if (_context == null)
                {
                    _context = new ConfigurationContext(_runtime.WorkingDirectory, "Rocket.NHibernate");
                    _config.Load(_context, new NHibernateProvidersConfiguration());
                }
                var conf = _config.Get<NHibernateProvidersConfiguration>();
                return conf.Connections.FirstOrDefault(c =>
                    c.ConnectionName.Equals(conf.DefaultConnection, StringComparison.OrdinalIgnoreCase))
                    ?? throw new Exception("Could not find connection settings for: " + conf.DefaultConnection);
            }
        }
    }

    public class NHibernateProvidersConfiguration
    {
        [Comment("The connection provider to use.")]
        public string DefaultConnection = "DefaultMySqlConnection";

        [ConfigArray(ElementName = "ConnectionProvider")]
        [Comment("For connection strings, please have a look at https://www.connectionstrings.com/.")]
        public NHibernateConnectionInfo[] Connections { get; set; } =
        {
            new NHibernateConnectionInfo
            {
                ConnectionName = "DefaultInMemoryConnection",
                ProviderName = "InMemory",
                ConnectionString = "MyDatabaseName"
            },
            new NHibernateConnectionInfo
            {
                ConnectionName = "DefaultMySqlConnection",
                ProviderName = "MySql",
                ConnectionString = "SERVER=localhost; DATABASE=myDataBase; UID=myUsername; PASSWORD=myPassword"
            },
            new NHibernateConnectionInfo
            {
                ConnectionName = "DefaultPostgreSqlConnection",
                ProviderName = "PostgreSql",
                ConnectionString = "User ID=root;Password=myPassword;Host=localhost;Port=5432;Database=myDataBase;"
            },
            new NHibernateConnectionInfo
            {
                ConnectionName = "DefaultSqliteConnection",
                ProviderName = "Sqlite",
                ConnectionString = "Data Source={PluginDir}\\Plugin.db;Version=3;"
            },
            new NHibernateConnectionInfo
            {
                ConnectionName = "DefaultSqlServerConnection",
                ProviderName = "SqlServer",
                ConnectionString = "Server=localhost;Database=myDataBase;User Id=myUsername;Password=myPassword;"
            },
            new NHibernateConnectionInfo
            {
                ConnectionName = "DefaultSqlServerCompactConnection",
                ProviderName = "SqlServerCompact",
                ConnectionString = "Data Source={PluginDir}\\Plugin.sdf;Persist Security Info=False;"
            }
        };
    }
}