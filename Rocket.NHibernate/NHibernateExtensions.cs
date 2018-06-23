using System;
using System.Collections.Generic;
using NHibernate;
using Rocket.API.Plugins;

namespace Rocket.NHibernate
{
    public static class NHibernateExtensions
    {
        public static INHibernateBuilder GetNHibernateBuilder(this IPlugin plugin)
        {
            var service = plugin.Container.Resolve<INHibernateService>();
            return new NHibernateBuilder(service, plugin);
        }


        public static ISession OpenSession(this IPlugin plugin)
        {
            return plugin.Container.Resolve<INHibernateService>().OpenSession(plugin);
        }

        public static void SaveEntry<T>(this ISession session, T entry) where T: class
        {
            using (var trans = session.BeginTransaction())
            {
                session.Save(entry);
                trans.Commit();
            }
        }

        public static IEnumerable<T> Query<T>(this ISession session) where T: class
        {
            return Query<T>(session, null);
        }

        public static IEnumerable<T> Query<T>(this ISession session, Func<ICriteria, ICriteria> action) where T : class
        {
            IEnumerable<T> enumerable;
            using (var trans = session.BeginTransaction())
            {
                var criteria = session.CreateCriteria<T>();
                if(action != null)
                    criteria = action(criteria);

                enumerable = criteria.List<T>();
                trans.Commit();
            }

            return enumerable;
        }

    }
}