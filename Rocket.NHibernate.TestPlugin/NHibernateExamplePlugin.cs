using System;
using System.Linq;
using FluentNHibernate.Mapping;
using NHibernate;
using Rocket.API.DependencyInjection;
using Rocket.Core.Plugins;

namespace Rocket.NHibernate.ExamplePlugin
{
    public class ExamplePlugin : Plugin, INHibernatePlugin
    {
        public ISession Session { get; protected set; }

        public ExamplePlugin(IDependencyContainer container) : base(container)
        {
        }

        protected override void OnLoad(bool isFromReload)
        {
            base.OnLoad(isFromReload);
            this.GetNHibernateBuilder()
                .UseAutoMapping() //Alternatively you can skip AutoMapping and use the TestEntryMap below
                .AddNHibernate();

            Session = this.OpenSession();

            var toAdd = new TestEntry
            {
                Name = Guid.NewGuid().ToString()
            };
            Logger.Log($"Adding: \"{toAdd.Name}\" to database");

            this.SaveEntry(toAdd);

            var entries = this.Query<TestEntry>().ToList();
            Logger.Log("Entry count: " + entries.Count);
            foreach (var entry in entries)
            {
                Logger.Log($"[{entry.Id}] \"{entry.Name}\"");
            }
        }

        public class TestEntry
        {
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
