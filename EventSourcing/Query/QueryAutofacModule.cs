using System.Linq;
using Autofac;
using EventSourcing.Query.UsingProjection;
using EventSourcing.Query.UsingSubscription;

namespace EventSourcing.Query
{
    public class QueryAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Because every instance also implements IStartable, I had to register
            // the instances like this and create sub classes for every type instead of RegisterGeneric.
            // Autofac does not support using RegisterGeneric of types that also implement IStartable.
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(MemoryRepository<,>))
                .AsImplementedInterfaces()
                .SingleInstance();


            builder.RegisterTypes(ThisAssembly.GetTypes()
                    .Where(type => type.BaseType == typeof(AbstractProjectionManager))
                    .ToArray()
                )
                .As<IStartable>()
                .SingleInstance();
        } 
    }
}