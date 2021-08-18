using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EventSourcing.Mediator
{
    public interface IMediator
    {
        Task<TResponse> Handle<TCommand, TResponse>(ICommand<TCommand,TResponse> command, CancellationToken cancellationToken)
            where TCommand : ICommand<TCommand, TResponse>;

        
        // Task<TResponse> Query<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken)
        //     where TQuery : IQuery<TQuery, TResponse>
        //     where TResponse : IResponse;
        //
        // Task<TLegacyResponse> Do<TLegacyCommand, TLegacyResponse>(
        //     ILegacyCommand<TLegacyCommand, TLegacyResponse> command, CancellationToken cancellationToken)
        //     where TLegacyCommand : ILegacyCommand<TLegacyCommand, TLegacyResponse>
        //     where TLegacyResponse : ILegacyResponse;
    }
}