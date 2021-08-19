using System;

namespace EventSourcing.EventSourcing
{
    public class Event
    {
        public Guid AggregateId { get; set; }
        public long Revision { get; } = -1;
        
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