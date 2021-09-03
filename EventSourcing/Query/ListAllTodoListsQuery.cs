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
    public class ListAllTodoListsQuery : IQuery<ListAllTodoListsQuery, IEnumerable<TodoListIndexItem>>
    {
    }

    public class ListAllTodoListsRepository : MemoryRepository<TodoListIndexItem, TodoList>
    {
        public ListAllTodoListsRepository(ILogger<MemoryRepository<TodoListIndexItem, TodoList>> logger,
            IEventStoreRepository<TodoList> eventStoreRepository) : base(logger, eventStoreRepository)
        {
        }
    }

    public class ListAllTodoListsQueryHandler : IQueryHandler<ListAllTodoListsQuery, IEnumerable<TodoListIndexItem>>
    {
        private readonly IRepository<TodoListIndexItem> _repository;

        public ListAllTodoListsQueryHandler(IRepository<TodoListIndexItem> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TodoListIndexItem>> Query(ListAllTodoListsQuery query,
            CancellationToken cancellationToken)
        {
            return await _repository.All();
        }
    }

    public class TodoListIndexItem : QueryModel
    {
        public Guid Id { get; set; }
        public int NumberOfTasks { get; set; } = 0;

        public void On(TodoListCreatedEvent evt)
        {
            Id = evt.AggregateId;
        }

        public void On(TodoItemAddedEvent evt)
        {
            NumberOfTasks++;
        }
    }
}