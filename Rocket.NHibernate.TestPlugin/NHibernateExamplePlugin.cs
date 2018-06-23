using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
//using FluentNHibernate.Mapping; // uncomment when using TestEntryMap at the bottom
using NHibernate;
using NHibernate.Criterion;
using Rocket.API.DependencyInjection;
using Rocket.Core.Plugins;

namespace Rocket.NHibernate.ExamplePlugin
{
    public class ExamplePlugin : Plugin
    {
        private ISession _session;

        public ExamplePlugin(IDependencyContainer container) : base(container)
        {
        }

        protected override void OnLoad(bool isFromReload)
        {
            base.OnLoad(isFromReload);
            this.GetNHibernateBuilder()
                .UseAutoMapping() //Alternatively you can skip AutoMapping and use the TestEntryMap below
                .AddNHibernate();

            _session = this.OpenSession();

            var toAdd = new TestEntry
            {
                Name = Guid.NewGuid().ToString()
            };
            Logger.Log($"Adding: \"{toAdd.Name}\" to database");

            _session.SaveEntry(toAdd);

            var entries = _session.Query<TestEntry>(c => c.AddOrder(Order.Asc("Id"))).ToList();
            Logger.Log("Entry count: " + entries.Count);
            foreach (var entry in entries)
            {
                Logger.Log($"[{entry.Id}] \"{entry.Name}\"");
            }
        }

        [Table("TestEntries")]
        public class TestEntry
        {
            [Key]
            public virtual int Id { get; set; }
            public virtual string Name { get; set; }
        }

        /* You can use this instead of UseAutoMapping():
        public class TestEntryMap : ClassMap<TestEntry>
        {
            public TestEntryMap()
            {
                Id(x => x.Id);
                Map(x => x.Name);
            }
        }
        */
    }
}
