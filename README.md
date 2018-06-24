# Rocket.NHibernate [![Build status](https://ci.appveyor.com/api/projects/status/wt5onkbdym9ajtpx?svg=true)](https://ci.appveyor.com/project/RocketMod/rocket-nhibernate/)

Provides NHibernate integrations for RocketMod .NET Game Server Plugin Framework.

To use in your plugins, add `Rocket.NHibernate` to your plugin from NuGet and then use in your Load(): 
```cs
this
  .GetNHibernateBuilder()
  //.UseAutoMapping()
  .AddNHibernate();
```