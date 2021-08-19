using System;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class AddTodoItemCommand : ICommand<AddTodoItemCommand, Guid>
    {
        public Guid TodoListId { get; set; }
        public Guid ItemId { get; set; }
        public string Description { get; set; }
    }
}