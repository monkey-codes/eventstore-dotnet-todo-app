using Autofac;
using EventSourcing.Query.Projection;

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

            builder.RegisterType<ProjectionManager>()
                .As<IStartable>()
                .SingleInstance();
        } 
    }
}