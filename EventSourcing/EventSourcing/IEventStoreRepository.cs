using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Mediator;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace EventSourcing.EventSourcing
{
    public interface IEventStoreRepository<T>
    where T : Aggregate
    {
        Task<long> Save(T todoList, long expectedRevision, CancellationToken cancellationToken);
        Task<T> Load(Guid aggregateId);

        Task Subscribe(Func<Event, CancellationToken, Task> eventHandler);
    }

    public class EventStoreRepository<T> : IEventStoreRepository<T>
    where T: Aggregate
    {
        private readonly EventStoreClient _client;
        private readonly ILogger<EventStoreRepository<T>> _logger;

        public EventStoreRepository(EventStoreClient client, ILogger<EventStoreRepository<T>> logger)
        {
            _client = client;
            _logger = logger;
        }
        public async Task<long> Save(T t, long expectedRevision, CancellationToken cancellationToken)
        {
            var eventDatas = t.Changes.Select(evt => new EventData(
                Uuid.NewUuid(),
                evt.EventType,
                JsonSerializer.SerializeToUtf8Bytes(evt,evt.GetType())
            ));
            
            var streamName = StreamName(t.GetType(), t.AggregateId);
            var revision =
                expectedRevision == -1 ? StreamRevision.None : StreamRevision.FromInt64(expectedRevision);
            return (await _client.AppendToStreamAsync(
                streamName,
                revision,
                eventDatas,
                cancellationToken: cancellationToken
            )).NextExpectedStreamRevision.ToInt64();
        }

        public async Task<T> Load(Guid aggregateId)
        {
            var type = typeof(T);
            var streamName = StreamName(type, aggregateId);
            var readStreamResult = _client.ReadStreamAsync(
                Direction.Forwards,
                streamName,
                StreamPosition.Start
            );
            if (await readStreamResult.ReadState == ReadState.StreamNotFound)
            {
                return null;
            }
            var events = await readStreamResult.Select(evt => ToEvent(evt))
                .ToListAsync();
            var instance = (T) Activator.CreateInstance(type, new [] {events});
            return instance;
        }

        public async Task Subscribe(Func<Event, CancellationToken, Task> eventHandler)
        {
            await _client.SubscribeToStreamAsync(
                StreamCategoryName(typeof(T)),
                // StreamPosition.Start,
                async (StreamSubscription subscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken) =>
                {
                    _logger.LogInformation($"Subscription received event: ${resolvedEvent.Event.EventType}");
                    await eventHandler(ToEvent(resolvedEvent), cancellationToken);
                },
                subscriptionDropped: SubscriptionDropped,
                resolveLinkTos: true
            );
        }

        private void SubscriptionDropped(StreamSubscription subscription, SubscriptionDroppedReason reason, Exception? arg3)
        {
            _logger.LogWarning(arg3,$"Subscription Dropped {reason}");
        }
        
        private Event ToEvent(ResolvedEvent resolvedEvent)
        {
            var eventEventType = resolvedEvent.Event.EventType;
            var eventJson = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
            var type = Type.GetType(eventEventType);
            var e = (Event) JsonSerializer.Deserialize(eventJson, type);
            e.Revision = StreamRevision.FromStreamPosition(resolvedEvent.Event.EventNumber).ToInt64();
            return e;
        }

        private static string StreamName(Type type, Guid tAggregateId)
        {
            return $"{type.Name}-{tAggregateId.ToString()}";
        }
        
        private static string StreamCategoryName(Type type)
        {
            return $"$ce-{type.Name}";
        }
    }
}