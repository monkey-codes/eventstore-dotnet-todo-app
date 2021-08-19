using Autofac;
using EventStore.Client;

namespace EventSourcing.EventSourcing
{
    public class EventSourcingAutofacModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //TODO extract url config to settings
            builder
                .Register(context => new EventStoreClient(
                    EventStoreClientSettings.Create(
                        "esdb://localhost:2113?tls=false")
                ))
                .SingleInstance();
            
            builder.RegisterGeneric(typeof(EventStoreRepository<>))
                .As(typeof(IEventStoreRepository<>))
                .InstancePerLifetimeScope();
        }
    }
}