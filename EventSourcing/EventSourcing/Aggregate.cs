
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using EventStore.Client;

namespace EventSourcing.EventSourcing
{
    public abstract class Aggregate
    {
        public Guid AggregateId { get; protected set; }
        
        protected long BaseRevision = -1;
        public List<Event> Changes { get; } = new List<Event>();

        public Aggregate(List<Event> events)
        {
            HydrateFromHistory(events);
        }

        public Aggregate() : this(new List<Event>())
        {
        }
        
        private void HydrateFromHistory(List<Event> events)
        {
            foreach (var @event in events)
            {
                Apply(@event);
                BaseRevision = @event.Revision;
            }
        }

        protected void ApplyChange(Event evt)
        {
            Apply(evt);
            Changes.Add(evt);
        }
        
        private void Apply(Event evt)
        {
            Invoke(evt, "On");
        }

        private void Invoke(object evt, string methodName)
        {
            GetType().GetMethod(methodName, new [] {evt.GetType()})?.Invoke(this, new [] {evt});
        }
    }
}