using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentNHibernate;
using FluentNHibernate.Automapping;

namespace Rocket.NHibernate
{
    public class RocketDefaultAutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool IsId(Member member)
        {
            if (base.IsId(member))
                return true;

            return member.MemberInfo.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0;
        }

        public override bool IsComponent(Type type)
        {
            if (base.IsComponent(type))
                return true;
            return type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0;
        }
    }
}