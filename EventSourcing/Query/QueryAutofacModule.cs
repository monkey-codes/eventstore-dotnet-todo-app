using Autofac;

namespace EventSourcing.Query
{
    public class QueryAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(MemoryRepository<,>))
                .AsImplementedInterfaces()
                .SingleInstance();
        } 
    }
}