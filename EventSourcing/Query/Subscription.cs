using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using EventStore.Client;

namespace EventSourcing.Query
{
    public interface ISubscription
    {
    }

    public class Subscription : ISubscription, IStartable
    {
        private readonly EventStoreClient _client;

        public Subscription(EventStoreClient client)
        {
            _client = client;
        }

        public Subscription()
        {
            Console.Out.WriteLine("CREATED");
        }

        public async void Start()
        {
            Console.Out.WriteLine("CREATED!");
            Console.Out.WriteLine(_client);
            var subscribeToStreamAsync = await _client.SubscribeToStreamAsync(
                "$ce-TodoList",
                // StreamPosition.Start,
                eventAppeared: EventAppeared,
                subscriptionDropped: SubscriptionDropped
            );
        }

        private void SubscriptionDropped(StreamSubscription arg1, SubscriptionDroppedReason arg2, Exception? arg3)
        {
            Console.Out.WriteLine("Dropped!");

        }

        private Task EventAppeared(StreamSubscription arg1, ResolvedEvent arg2, CancellationToken arg3)
        {
            Console.Out.WriteLine($"Event received: {arg2.GetType().Name}");
            // throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}