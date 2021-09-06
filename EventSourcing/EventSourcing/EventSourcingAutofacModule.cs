using Autofac;
using EventStore.Client;

namespace EventSourcing.EventSourcing
{
    public class EventSourcingAutofacModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //TODO extract url config to settings
            var connection = "esdb://localhost:2113?tls=false";
            builder
                .Register(context => new EventStoreClient(
                    EventStoreClientSettings.Create(connection)
                ))
                .SingleInstance();

            builder.Register(context =>
            {
                var settings = EventStoreClientSettings.Create(connection);
                settings.ConnectionName = "Projection management client";
                return new EventStoreProjectionManagementClient(settings); 
            });
            builder.RegisterGeneric(typeof(EventStoreRepository<>))
                .As(typeof(IEventStoreRepository<>))
                .InstancePerLifetimeScope();
        }
    }
}