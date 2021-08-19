using System;
using EventSourcing.EventSourcing;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class CreateTodoListCommand : CommandBase<CreateTodoListCommand, RevisionedResponse<object>>
    {
    }
}