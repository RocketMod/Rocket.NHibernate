using NHibernate;
using Rocket.API.Plugins;

namespace Rocket.NHibernate
{
    public interface INHibernatePlugin : IPlugin
    {
        ISession Session { get; }   
    }
}