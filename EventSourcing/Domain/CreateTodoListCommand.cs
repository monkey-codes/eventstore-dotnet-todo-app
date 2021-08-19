using System;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class CreateTodoListCommand : CommandBase<CreateTodoListCommand, long>
    {
        public Guid Id { get; set; }
    }
}