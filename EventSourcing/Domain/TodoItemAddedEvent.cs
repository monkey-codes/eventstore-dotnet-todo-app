using System;
using EventSourcing.EventSourcing;

namespace EventSourcing.Domain
{
    public class TodoItemAddedEvent : Event
    {
        public Guid ItemId { get; set; }
        public string Description { get; set; }
    }
}