using System;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class AddTodoItemCommand : CommandBase<AddTodoItemCommand, long>
    {
        public Guid ItemId { get; set; }
        public string Description { get; set; }
    }
}