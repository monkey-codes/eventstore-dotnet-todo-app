using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Domain;
using EventSourcing.EventSourcing;
using EventSourcing.Mediator;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Query
{
    public class GetTodoListQuery : IQuery<GetTodoListQuery, TodoListQueryModel>
    {
        public Guid Id { get; set; }
    }

    public class GetTodoListRepository : MemoryRepository<TodoListQueryModel, TodoList>
    {
        public GetTodoListRepository(ILogger<MemoryRepository<TodoListQueryModel, TodoList>> logger, IEventStoreRepository<TodoList> eventStoreRepository) : base(logger, eventStoreRepository)
        {
        }
    }
        
    public class GetTodoListQueryHandler : IQueryHandler<GetTodoListQuery, TodoListQueryModel>
    {
        private readonly IRepository<TodoListQueryModel> _repository;

        public GetTodoListQueryHandler(IRepository<TodoListQueryModel> repository)
        {
            _repository = repository;
        }

        public async Task<TodoListQueryModel> Query(GetTodoListQuery query,
            CancellationToken cancellationToken)
        {
            return await _repository.Load(query.Id);
        }
    }

    public class TodoListQueryModel : QueryModel
    {
        public Guid Id { get; set; }

        public List<TodoItem> Tasks { get; set; } = new List<TodoItem>();

        public void On(TodoListCreatedEvent evt)
        {
            Id = evt.AggregateId;
        }

        public void On(TodoItemAddedEvent evt)
        {
            Tasks.Add(new TodoItem(evt));
        }
    }

    public class TodoItem
    {
        public Guid TaskId { get; set; }
        public string Description { get; set; }

        public TodoItem(TodoItemAddedEvent evt)
        {
            TaskId = evt.ItemId;
            Description = evt.Description;
        }
    }
}