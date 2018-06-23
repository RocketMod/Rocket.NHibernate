using Rocket.API.DependencyInjection;
using Rocket.Core.Plugins;

namespace Rocket.NHibernate
{
    public class NHibernatePlugin : Plugin
    {
        public NHibernatePlugin(IDependencyContainer container) : base("Rocket.NHibernate", container)
        {
        }
    }
}