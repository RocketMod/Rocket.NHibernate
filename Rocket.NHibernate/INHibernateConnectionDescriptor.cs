namespace Rocket.NHibernate
{
    public interface INHibernateConnectionDescriptor
    {
        NHibernateConnectionInfo ConnectionInfo { get; }
    }
}