
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

        public object Handle(object command)
        {
            return Invoke(command, "Handle");
        }
        
        private void Apply(Event evt)
        {
            Invoke(evt, "On");
        }

        private object Invoke(object evt, string methodName)
        {
            return GetType().GetMethod(methodName, new [] {evt.GetType()})?.Invoke(this, new [] {evt});
        }
    }
}