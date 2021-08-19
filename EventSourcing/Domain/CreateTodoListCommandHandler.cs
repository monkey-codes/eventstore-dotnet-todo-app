using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.EventSourcing;
using EventSourcing.Mediator;

namespace EventSourcing.Domain
{
    public class CreateTodoListCommandHandler : ICommandHandler<CreateTodoListCommand, long>
    {
        private readonly IEventStoreRepository<TodoList> _repository;

        public CreateTodoListCommandHandler(IEventStoreRepository<TodoList> repository)
        {
            _repository = repository;
        }

        public async Task<long> Handle(CreateTodoListCommand command, CancellationToken cancellationToken)
        {
            var todoList = await _repository.Load(command.Id);
            if (todoList == null)
            {
                todoList = new TodoList(command);
            }
            // var todoList = new TodoList(command);
            var revision = await _repository.Save(todoList, command.ExpectedRevision, cancellationToken);
            return revision;
        }
    }

    public class AddTodoItemCommandHandler : ICommandHandler<AddTodoItemCommand, long>
    {
        private readonly IEventStoreRepository<TodoList> _repository;

        public AddTodoItemCommandHandler(IEventStoreRepository<TodoList> repository)
        {
            _repository = repository;
        }
        public async Task<long> Handle(AddTodoItemCommand command, CancellationToken cancellationToken)
        {
            var todoList = await _repository.Load(command.TodoListId);
            todoList.Handle(command);
            var revision = await _repository.Save(todoList, command.ExpectedRevision, cancellationToken);
            return revision;
        }
    }
}