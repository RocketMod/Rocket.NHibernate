using FluentNHibernate.Automapping;
using Rocket.API.Plugins;

namespace Rocket.NHibernate
{
    public class NHibernateBuilder : INHibernateBuilder
    {
        private readonly INHibernateService _service;
        private readonly IPlugin _plugin;
        public bool AutoMap { get; protected set; }

        public NHibernateBuilder(INHibernateService service, IPlugin plugin)
        {
            _service = service;
            _plugin = plugin;
        }
        public virtual INHibernateBuilder UseAutoMapping(bool value = true)
        {
            if (AutoMap)
                return this;

            AutoMap = value;
            AutoMapConfiguration = new RocketDefaultAutomappingConfiguration();
            return this;
        }

        public INHibernateBuilder UseAutoMapping(IAutomappingConfiguration configuration)
        {
            AutoMap = true;
            AutoMapConfiguration = configuration;
            return this;
        }

        public IAutomappingConfiguration AutoMapConfiguration { get; set; }

        public virtual void AddNHibernate()
        {
            _service.InitializeSessionFactory(_plugin, this);
        }
    }
}