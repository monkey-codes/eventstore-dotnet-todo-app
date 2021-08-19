using System;
using System.Windows.Input;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class CreateTodoListCommand : ICommand<CreateTodoListCommand, Guid>
    {
        public Guid Id { get; set; }
    }
}