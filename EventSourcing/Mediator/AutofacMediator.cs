using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Mediator
{
    public class AutofacMediator : IMediator
    {
        private readonly ILifetimeScope _scope;
        private readonly ILogger<AutofacMediator> _logger;

        public AutofacMediator(ILifetimeScope scope, ILogger<AutofacMediator> logger)
        {
            _scope = scope;
            _logger = logger;
        }
        
        public async Task<TResponse> Handle<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken) 
            where TCommand : ICommand<TCommand, TResponse>
        {
            var handler = _scope.Resolve<ICommandHandler<TCommand, TResponse>>();
            _logger.LogInformation($"{{Handler}} handling {{Command}}",
                handler.GetType().ToString(), command.GetType().ToString());
            return await handler.Handle((TCommand)command, cancellationToken);
        }

        public async Task<TResponse> Query<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken) where TQuery : IQuery<TQuery, TResponse>
        {
            var handler = _scope.Resolve<IQueryHandler<TQuery, TResponse>>();
            _logger.LogInformation($"{{Handler}} handling {{Query}}",
                handler.GetType().ToString(), query.GetType().ToString());
            return await handler.Query((TQuery)query, cancellationToken);
        }
    }
}