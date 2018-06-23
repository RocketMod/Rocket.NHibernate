using FluentNHibernate.Automapping;

namespace Rocket.NHibernate
{
    public interface INHibernateBuilder
    {
        /// <summary>
        /// See https://github.com/FluentNHibernate/fluent-nhibernate/wiki/Auto-mapping
        /// </summary>
        INHibernateBuilder UseAutoMapping(bool value = true);
        INHibernateBuilder UseAutoMapping(IAutomappingConfiguration configuration);
        void AddNHibernate();
    }
}