using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace EventSourcing.EventSourcing
{
    public interface IEventStoreRepository<T>
    where T : Aggregate
    {
        Task Save(T todoList, CancellationToken cancellationToken);
        Task<T> Load(Guid aggregateId);
    }

    public class EventStoreRepository<T> : IEventStoreRepository<T>
    where T: Aggregate
    {
        private readonly EventStoreClient _client;

        public EventStoreRepository(EventStoreClient client)
        {
            _client = client;
        }
        public async Task Save(T t, CancellationToken cancellationToken)
        {
            var eventDatas = t.Changes.Select(evt => new EventData(
                Uuid.NewUuid(),
                evt.EventType,
                JsonSerializer.SerializeToUtf8Bytes(evt,evt.GetType())
            ));
            var streamName = StreamName(t.GetType(), t.AggregateId);
            await _client.AppendToStreamAsync(
                streamName,
                StreamState.Any,
                eventDatas,
                cancellationToken: cancellationToken
            );
        }

        public async Task<T> Load(Guid aggregateId)
        {
            var type = typeof(T);
            var streamName = StreamName(type, aggregateId);
            var events = await _client.ReadStreamAsync(
                Direction.Forwards,
                streamName,
                StreamPosition.Start
            ).Select(evt => ToEvent(evt))
                .ToListAsync();
            var instance = (T) Activator.CreateInstance(type, new [] {events});
            return instance;
        }

        private Event ToEvent(ResolvedEvent resolvedEvent)
        {
            var eventEventType = resolvedEvent.Event.EventType;
            var eventJson = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
            var type = Type.GetType(eventEventType);
            var e = (Event) JsonSerializer.Deserialize(eventJson, type);
            return e;
        }

        private static string StreamName(Type type, Guid tAggregateId)
        {
            return $"{type.Name}-{tAggregateId.ToString()}";
        }
    }
}