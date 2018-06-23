using Rocket.API.DependencyInjection;

namespace Rocket.NHibernate.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<INHibernateConnectionDescriptor, NHibernateConnectionDescriptor>();
            container.RegisterSingletonType<INHibernateService, NHibernateService>();
        }
    }
}