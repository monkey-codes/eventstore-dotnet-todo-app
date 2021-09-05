using System;
using System.Collections.Generic;
using EventSourcing.EventSourcing;

namespace EventSourcing.Domain
{
    public class TodoList : Aggregate
    {
        private List<TodoItem> Items { get; } = new List<TodoItem>();
        public TodoList(CreateTodoListCommand command)
        {
            ApplyChange(new TodoListCreatedEvent
            {
                AggregateId = command.Id
            });
        }

        public TodoList(List<Event> events) : base(events)
        {
        }

        public void Handle(AddTodoItemCommand command)
        {
            ApplyChange(
                new TodoItemAddedEvent
                {
                    AggregateId = command.Id,
                    ItemId = command.ItemId,
                    Description = command.Description
                }
            );
        }

        public void On(TodoListCreatedEvent evt)
        {
            AggregateId = evt.AggregateId;
        }

        public void On(TodoItemAddedEvent evt)
        {
            Items.Add(new TodoItem
            {
                Id = evt.ItemId,
                Description = evt.Description
            });
        }
    }

    internal class TodoItem
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}