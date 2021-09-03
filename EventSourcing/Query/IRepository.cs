using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using EventSourcing.EventSourcing;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Query
{
    
    public interface IRepository<TQueryModel>
    {
        public Task<TQueryModel> Load(Guid id);

        public Task<IEnumerable<TQueryModel>> All();
    }

    public abstract class QueryModel
    {
        public long Revision { get; set; }
    }
    
    public abstract class MemoryRepository<TQueryModel, TAggregate> : IRepository<TQueryModel>, IStartable
    where TAggregate: Aggregate
    where TQueryModel: QueryModel
    {
        private readonly ILogger<MemoryRepository<TQueryModel, TAggregate>> _logger;
        private readonly IEventStoreRepository<TAggregate> _eventStoreRepository;
        private readonly Dictionary<Guid, TQueryModel> _memoryStore = new Dictionary<Guid, TQueryModel>();

        protected MemoryRepository(ILogger<MemoryRepository<TQueryModel, TAggregate>> logger, IEventStoreRepository<TAggregate> eventStoreRepository)
        {
            _logger = logger;
            _eventStoreRepository = eventStoreRepository;
        }
        
        public async void Start()
        {
            _logger.LogDebug($"Starting {GetType().Name} Subscription");
            await _eventStoreRepository.Subscribe(EventHandler);
        }

        public Task<TQueryModel> Load(Guid id)
        {
            return Task.FromResult(_memoryStore[id]);
        }

        public Task<IEnumerable<TQueryModel>> All()
        {
            return Task.FromResult(_memoryStore.Values.AsEnumerable());
        }

        private Task EventHandler(Event evt, CancellationToken arg2)
        {
            _logger.LogDebug($"Received {evt.GetType()}");
            if (!_memoryStore.ContainsKey(evt.AggregateId))
            {
                _memoryStore[evt.AggregateId] = Activator.CreateInstance<TQueryModel>();
            }

            var queryModel = _memoryStore[evt.AggregateId];
            queryModel.GetType().GetMethod("On", new[] {evt.GetType()})
                ?.Invoke(queryModel, new[] {evt});
            queryModel.Revision = evt.Revision;
            return Task.CompletedTask;
        }
        
    }
}