using System;
using System.Collections.Generic;
using NHibernate;
using Rocket.API.Plugins;

namespace Rocket.NHibernate
{
    public static class NHibernateExtensions
    {
        public static INHibernateBuilder GetNHibernateBuilder(this INHibernatePlugin plugin)
        {
            var service = plugin.Container.Resolve<INHibernateService>();
            return new NHibernateBuilder(service, plugin);
        }


        public static ISession OpenSession(this INHibernatePlugin plugin)
        {
            return plugin.Container.Resolve<INHibernateService>().OpenSession(plugin);
        }

        public static void SaveEntry<T>(this INHibernatePlugin plugin, T entry) where T: class
        {
            using (var trans = plugin.Session.BeginTransaction())
            {
                plugin.Session.Save(entry);
                trans.Commit();
            }
        }

        public static IEnumerable<T> Query<T>(this INHibernatePlugin plugin) where T: class
        {
            return Query<T>(plugin, null);
        }

        public static IEnumerable<T> Query<T>(this INHibernatePlugin plugin, Func<ICriteria, ICriteria> action) where T : class
        {
            IEnumerable<T> enumerable;
            using (var trans = plugin.Session.BeginTransaction())
            {
                var criteria = plugin.Session.CreateCriteria<T>();
                if(action != null)
                    criteria = action(criteria);

                enumerable = criteria.List<T>();
                trans.Commit();
            }

            return enumerable;
        }

    }
}