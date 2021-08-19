using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.EventSourcing;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class AddTodoItemCommandHandler : GenericCommandHandler<AddTodoItemCommand, TodoList>
    {
        public AddTodoItemCommandHandler(IEventStoreRepository<TodoList> repository) : base(repository)
        {
        }
    }
    
    public class CreateTodoListCommandHandler: GenericCommandHandler<CreateTodoListCommand, TodoList>
    {
        public CreateTodoListCommandHandler(IEventStoreRepository<TodoList> repository) : base(repository)
        {
        }
    }

    public abstract class GenericCommandHandler<TCommand, TAggregateType> : ICommandHandler<TCommand, long>
        where TCommand : ICommand<TCommand, long>
        where TAggregateType : Aggregate
    {
        private readonly IEventStoreRepository<TAggregateType> _repository;

        public GenericCommandHandler(IEventStoreRepository<TAggregateType> repository)
        {
            _repository = repository;
        }
        public async Task<long> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var aggregate = await _repository.Load(command.Id);
            if (aggregate == null)
            {
                var type = typeof(TAggregateType);
                aggregate = (TAggregateType) Activator.CreateInstance(type, command);
            }
            else
            {
                aggregate.Handle(command);
            }
 
            var revision = await _repository.Save(aggregate, command.ExpectedRevision, cancellationToken);
            return revision;
        }
    }
}