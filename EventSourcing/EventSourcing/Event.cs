using System;
using System.Text.Json.Serialization;

namespace EventSourcing.EventSourcing
{
    public class Event
    {
        public Guid AggregateId { get; set; }

        [JsonIgnore] 
        public long Revision { get; set; } = -1;
        
        public DateTimeOffset Created { get; } = DateTimeOffset.UtcNow;

        public string EventType => this.GetType().FullName;

        public Event(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Event()
        {
            // used by deserialization
        }
        
    }
}