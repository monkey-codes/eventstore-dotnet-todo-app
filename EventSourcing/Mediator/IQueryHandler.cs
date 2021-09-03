using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Mediator
{
    public interface IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TQuery, TResponse>
    {
        Task<TResponse> Query(TQuery query, CancellationToken cancellationToken);
    }
}