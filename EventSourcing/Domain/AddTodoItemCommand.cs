using System;
using EventSourcing.EventSourcing;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class AddTodoItemCommand : CommandBase<AddTodoItemCommand, RevisionedResponse<object>>
    {
        public Guid ItemId { get; set; }
        public string Description { get; set; }
    }
}