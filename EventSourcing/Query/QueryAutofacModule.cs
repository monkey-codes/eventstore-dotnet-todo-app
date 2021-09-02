using Autofac;

namespace EventSourcing.Query
{
    public class QueryAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            // builder.Regis
            builder
                .RegisterType<Subscription>()
                .As<IStartable>()
                .SingleInstance();
        } 
    }
}