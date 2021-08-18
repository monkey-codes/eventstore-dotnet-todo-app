using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class CreateTodoListCommandHandler : ICommandHandler<CreateTodoListCommand, Guid>
    {
        public Task<Guid> Handle(CreateTodoListCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(command.Id);
        }
    }
}