using Autofac;

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

            builder.RegisterType<Hello>().As<IHello>()
                .InstancePerLifetimeScope();
        }
    }
}