using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Mediator;

namespace EventSourcing.EventSourcing
{
    public abstract class GenericCommandHandler<TCommand, TAggregateType, TResponse> : ICommandHandler<TCommand, RevisionedResponse<TResponse>>
        where TCommand : ICommand<TCommand, RevisionedResponse<TResponse>>
        where TAggregateType : Aggregate
    {
        private readonly IEventStoreRepository<TAggregateType> _repository;

        public GenericCommandHandler(IEventStoreRepository<TAggregateType> repository)
        {
            _repository = repository;
        }
        public async Task<RevisionedResponse<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var aggregate = await _repository.Load(command.Id);
            TResponse response = default;
            if (aggregate == null)
            {
                var type = typeof(TAggregateType);
                aggregate = (TAggregateType) Activator.CreateInstance(type, command);
            }
            else
            {
                response =  (TResponse) aggregate.Handle(command);
            }
 
            var revision = await _repository.Save(aggregate, command.ExpectedRevision, cancellationToken);
            return new RevisionedResponse<TResponse>
            {
                Revision = revision,
                Response = response
            };
        }
    }
    public class RevisionedResponse<TResponse>
    {
        public long Revision { get; set; }
        public TResponse Response { get; set; }
    }
}