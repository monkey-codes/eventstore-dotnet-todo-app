using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using EventSourcing.Domain;
using EventSourcing.EventSourcing;
using EventSourcing.Mediator;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Query
{
    public class ListAllTodoListsQuery : IQuery<ListAllTodoListsQuery, IEnumerable<TodoListIndexItem>>
    {
        
    }
    
    public class TodoListsRepository : IStartable, IQueryHandler<ListAllTodoListsQuery, IEnumerable<TodoListIndexItem>>
    {
        private readonly ILogger<TodoListsRepository> _logger;
        private readonly IEventStoreRepository<TodoList> _eventStoreRepository;
        private readonly Dictionary<Guid, TodoListIndexItem> _memoryStore = new Dictionary<Guid, TodoListIndexItem>();
        public TodoListsRepository(ILogger<TodoListsRepository> logger, IEventStoreRepository<TodoList> eventStoreRepository)
        {
            _logger = logger;
            _eventStoreRepository = eventStoreRepository;
        }
        
        public async void Start()
        {
            _logger.LogInformation($"Starting {GetType().Name} Subscription");
            await _eventStoreRepository.Subscribe(EventHandler);
        }

        private Task EventHandler(Event evt, CancellationToken arg2)
        {
            _logger.LogInformation($"Received {evt.GetType()}");
            if (!_memoryStore.ContainsKey(evt.AggregateId))
            {
                _memoryStore[evt.AggregateId] = new TodoListIndexItem();
            }

            var queryModel = _memoryStore[evt.AggregateId];
            queryModel.GetType().GetMethod("On", new[] {evt.GetType()})
                .Invoke(queryModel, new[] {evt});
            queryModel.Revision = evt.Revision;
            return Task.CompletedTask;
        }
        
        private object Invoke(object evt, string methodName)
        {
            return GetType().GetMethod(methodName, new [] {evt.GetType()})?.Invoke(this, new [] {evt});
        }

        public Task<IEnumerable<TodoListIndexItem>> Query(ListAllTodoListsQuery command, CancellationToken cancellationToken)
        {
            return Task.FromResult(_memoryStore.Values.AsEnumerable());
        }
    }

    public class TodoListIndexItem
    {
        public Guid Id { get; set; }
        public long Revision { get; set; }
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