using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Domain;
using EventSourcing.EventSourcing;
using EventSourcing.Mediator;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Query.UsingSubscription
{
    public class ListAllTodoListsUsingSubscriptionQuery : IQuery<ListAllTodoListsUsingSubscriptionQuery, IEnumerable<TodoListIndexItem>>
    {
    }

    public class ListAllTodoListsRepository : MemoryRepository<TodoListIndexItem, TodoList>
    {
        public ListAllTodoListsRepository(ILogger<MemoryRepository<TodoListIndexItem, TodoList>> logger,
            IEventStoreRepository<TodoList> eventStoreRepository) : base(logger, eventStoreRepository)
        {
        }
    }

    public class ListAllTodoListsUsingSubscriptionQueryHandler : IQueryHandler<ListAllTodoListsUsingSubscriptionQuery, IEnumerable<TodoListIndexItem>>
    {
        private readonly IRepository<TodoListIndexItem> _repository;

        public ListAllTodoListsUsingSubscriptionQueryHandler(IRepository<TodoListIndexItem> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TodoListIndexItem>> Query(ListAllTodoListsUsingSubscriptionQuery query,
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