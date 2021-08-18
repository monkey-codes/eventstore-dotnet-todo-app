using Autofac;

namespace EventSourcing.Mediator
{
    public class MediatorAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // builder.RegisterType<MyService>().As<IService>();
            //
            // // Other Lifetime
            // // Transient
            // builder.RegisterType<MyService>().As<IService>()
            //     .InstancePerDependency();
            //
            // // Scoped
            // builder.RegisterType<MyService>().As<IService>()
            //     .InstancePerLifetimeScope();
            //
            // builder.RegisterType<MyService>().As<IService>()
            //     .InstancePerRequest();
            //
            // // Singleton
            // builder.RegisterType<MyService>().As<IService>()
            //     .SingleInstance();

            // Scan an assembly for components
            //builder.RegisterAssemblyTypes(typeof(Startup).Assembly)
            //       .Where(t => t.Name.EndsWith("Service"))
            //       .AsImplementedInterfaces();
            builder.RegisterType<Hello>().As<IHello>()
                .InstancePerLifetimeScope();
        }
    }
}