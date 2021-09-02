using Autofac;
using EventSourcing.Domain;
using EventSourcing.EventSourcing;

namespace EventSourcing.Mediator
{
    public class MediatorAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            
            // Scan an assembly for components
            // builder.RegisterAssemblyTypes(typeof(Startup).Assembly)
            //     .Where(t => t.Name.EndsWith("Service"))
            //     .AsImplementedInterfaces();
            
            builder.RegisterType<AutofacMediator>().As<IMediator>()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(ICommandHandler<,>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(IQueryHandler<,>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            
            // builder.RegisterGeneric(typeof(GenericCommandHandler<,>))
            //     // .As(typeof(ICommandHandler<,>))
            //     .InstancePerLifetimeScope();
            
        }
    }
}