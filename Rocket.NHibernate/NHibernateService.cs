﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Rocket.API;
using Rocket.API.Eventing;
using Rocket.API.Plugins;
using Rocket.Core.Plugins.Events;

namespace Rocket.NHibernate
{
    public class NHibernateService : INHibernateService, IEventListener<PluginUnloadEvent>
    {
        private readonly Dictionary<IPlugin, ISessionFactory> _sessionFactories = new Dictionary<IPlugin, ISessionFactory>();
        private readonly Dictionary<IPlugin, List<ISession>> _sessions = new Dictionary<IPlugin, List<ISession>>();

        public NHibernateService(IRuntime runtime, IEventManager eventManager)
        {
            eventManager.AddEventListener(runtime, this);
        }

        public ISessionFactory GetSessionFactory(IPlugin plugin)
        {
            if (_sessionFactories.ContainsKey(plugin))
                return _sessionFactories[plugin];
            throw new Exception(
                $"Could not find session factory for plugin: {plugin.Name}! Did you forget to call AddNHibernate()?");
        }

        public void InitializeSessionFactory(IPlugin plugin, NHibernateBuilder builder)
        {
            if (_sessionFactories.ContainsKey(plugin))
                return;

            var desc = plugin.Container.Resolve<INHibernateConnectionDescriptor>();

            var sessionFactory = Fluently.Configure()
                .Database(GetConfigurer(desc.ConnectionInfo))
                .Mappings(m =>
                    {
                        var pluginAssembly = plugin.GetType().Assembly;
                        m.FluentMappings
                            .AddFromAssembly(typeof(NHibernateService).Assembly)
                            .AddFromAssembly(pluginAssembly);

                        if (builder.AutoMap)
                        {
                            m.AutoMappings
                                .Add(builder.AutoMapConfiguration == null
                                    ? AutoMap.Assembly(pluginAssembly)
                                    : AutoMap.Assembly(pluginAssembly, builder.AutoMapConfiguration));
                        }
                    }
                 )
                .ExposeConfiguration(
                    cfg => new SchemaExport(cfg)
                    .Create(true, true))
                .BuildSessionFactory();

            _sessionFactories.Add(plugin, sessionFactory);
        }

        public virtual ISession OpenSession(IPlugin plugin)
        {
            var session = GetSessionFactory(plugin).OpenSession();

            if (!_sessions.ContainsKey(plugin))
                _sessions.Add(plugin, new List<ISession>());

            _sessions[plugin].Add(session);
            return session;
        }

        public virtual void CloseSession(IPlugin plugin, ISession session)
        {
            if (!_sessions.ContainsKey(plugin))
                return;

            session.Close();
            _sessions[plugin].RemoveAll(c => c == session);
        }

        public virtual IPersistenceConfigurer GetConfigurer(NHibernateConnectionInfo info)
        {
            switch (info.ProviderName.ToLower())
            {
                case "mysql":
                    return MySQLConfiguration.Standard
                        .ConnectionString(info.ConnectionString)
                        .ShowSql();
                case "sqlserver":
                    return MsSqlConfiguration.MsSql2012
                        .ConnectionString(info.ConnectionString)
                        .ShowSql();
                case "sqlservercompact":
                case "sqlserverce":
                    return MsSqlCeConfiguration.MsSqlCe40
                        .ConnectionString(info.ConnectionString)
                        .ShowSql();
                case "oracle":
                    return OracleDataClientConfiguration.Oracle10
                        .ConnectionString(info.ConnectionString)
                        .ShowSql();
                case "postgresql":
                    return PostgreSQLConfiguration.Standard
                        .ConnectionString(info.ConnectionString)
                        .ShowSql();
                case "jetdriver":
                    return JetDriverConfiguration.Standard
                        .ConnectionString(info.ConnectionString)
                        .ShowSql();
                case "ifxsql":
                    return IfxOdbcConfiguration.Informix1000
                        .ConnectionString(info.ConnectionString)
                        .ShowSql();
                case "sqlite":
                    return SQLiteConfiguration.Standard
                        .ConnectionString(info.ConnectionString)
                        .ShowSql();
                case "sqlanywhere":
                    return SQLAnywhereConfiguration.SQLAnywhere12
                        .ConnectionString(info.ConnectionString)
                        .ShowSql();
            }

            throw new Exception($"No database provider with name: {info.ProviderName} was found!");
        }

        public void HandleEvent(IEventEmitter emitter, PluginUnloadEvent @event)
        {
            if (!_sessions.ContainsKey(@event.Plugin))
                return;

            foreach(var sess in _sessions[@event.Plugin].ToList())
                CloseSession(@event.Plugin, sess);
        }
    }
}