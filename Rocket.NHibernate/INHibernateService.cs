using NHibernate;
using Rocket.API.Plugins;

namespace Rocket.NHibernate
{
    public interface INHibernateService
    {
        ISession OpenSession(IPlugin plugin);
        void InitializeSessionFactory(IPlugin plugin, NHibernateBuilder nHibernateBuilder);
        ISessionFactory GetSessionFactory(IPlugin plugin);
    }
}