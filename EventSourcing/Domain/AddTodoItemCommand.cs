using System;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class AddTodoItemCommand : CommandBase<AddTodoItemCommand, long>
    {
        public Guid TodoListId { get; set; }
        public Guid ItemId { get; set; }
        public string Description { get; set; }
    }
}