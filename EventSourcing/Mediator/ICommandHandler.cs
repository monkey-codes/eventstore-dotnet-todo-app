using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Mediator
{
    public interface ICommandHandler<TCommand, TResponse> 
        where TCommand : ICommand<TCommand, TResponse>
    {
        Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
    }
}