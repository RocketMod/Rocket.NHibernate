using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
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



        public static IEnumerable<T> QueryCriteriaWithTransaction<T>(this ISession session, Func<ICriteria, ICriteria> action) where T : class
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

        public static int DeleteWithTransaction<T>(this ISession session, Func<IQueryable<T>, IQueryable<T>> action) where T : class
        {
            int c = 0;
            using (var trans = session.BeginTransaction())
            {
                var queryable = session.Query<T>();

                if (action != null)
                    queryable = action(queryable);

                foreach (var t in queryable.ToList())
                {
                    session.Delete(t);
                    c++;
                }

                trans.Commit();
            }

            return c;
        }

        public static int UpdateWithTransaction<T>(this ISession session, params object[] toUpdate) where T : class
        {
            int c = 0;
            using (var trans = session.BeginTransaction())
            {
                foreach (var t in toUpdate)
                {
                    session.Update(t);
                    c++;
                }

                trans.Commit();
            }

            return c;
        }
    }
}