using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.EventSourcing;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class CreateTodoListCommandHandler : ICommandHandler<CreateTodoListCommand, Guid>
    {
        private readonly IEventStoreRepository<TodoList> _repository;

        public CreateTodoListCommandHandler(IEventStoreRepository<TodoList> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateTodoListCommand command, CancellationToken cancellationToken)
        {
            var todoList = new TodoList(command);
            await _repository.Save(todoList, cancellationToken);
            return command.Id;
        }
    }

    public class AddTodoItemCommandHandler : ICommandHandler<AddTodoItemCommand, Guid>
    {
        private readonly IEventStoreRepository<TodoList> _repository;

        public AddTodoItemCommandHandler(IEventStoreRepository<TodoList> repository)
        {
            _repository = repository;
        }
        public async Task<Guid> Handle(AddTodoItemCommand command, CancellationToken cancellationToken)
        {
            var todoList = await _repository.Load(command.TodoListId);
            todoList.Handle(command);
            await _repository.Save(todoList, cancellationToken);
            return todoList.AggregateId;
        }
    }
}