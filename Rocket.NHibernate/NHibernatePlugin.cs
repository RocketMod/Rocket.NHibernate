using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using Rocket.API.DependencyInjection;
using Rocket.Core.Plugins;

namespace Rocket.NHibernate
{
    public class NHibernatePlugin : Plugin
    {
        public NHibernatePlugin(IDependencyContainer container) : base("Rocket.NHibernate", container)
        {
        }

        protected override void OnLoad(bool isFromReload)
        {
            base.OnLoad(isFromReload);

            RegisterDbProvider(
                "MySQL Data Provider",
                ".Net Framework Data Provider for MySQL",
                "MySql.Data.MySqlClient",
                "MySql.Data.MySqlClient.MySqlClientFactory, MySqlConnector, Version=0.40.4.0, Culture=neutral, PublicKeyToken=d33d3e53aa5f8c92",
                "MySqlConnector.dll");
        }

        public bool RegisterDbProvider(string name, string description, string invariant, string type, string driverAssembly)
        {
            var path = Path.Combine(WorkingDirectory, "Drivers");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var driverFile = Path.Combine(path, driverAssembly);
            if (!File.Exists(driverFile))
                throw new Exception("File not found: " + driverFile);

            Assembly.LoadFile(driverFile);

            DataSet ds = ConfigurationManager.GetSection("system.data") as DataSet;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["InvariantName"].ToString() == invariant)
                {
                    Logger.Log("Found");
                    return true;
                }
            }

            Logger.Log("Not found; adding.");
            ds.Tables[0].Rows.Add(name, description, invariant, type);
            return true;
        }
    }
}