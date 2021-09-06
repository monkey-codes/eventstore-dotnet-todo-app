using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using EventSourcing.Mediator;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Query.Projection
{
    // fromStream('$ce-TodoList').when({
    // $init: function(){
    //     return {};
    // },
    // "EventSourcing.Domain.TodoListCreatedEvent": function(s,e){
    //     s[e.data.AggregateId] = {id: e.data.AggregateId, items: []};
    // },
    // "EventSourcing.Domain.TodoItemAddedEvent": function(s,e) {
    //     s[e.data.AggregateId].items.push({itemId: e.data.ItemId, description: e.data.Description })
    // }
    // }).outputState();
    public class ProjectionManager : IStartable
    {
        private readonly EventStoreProjectionManagementClient _client;
        private readonly ILogger<ProjectionManager> _logger;

        public ProjectionManager(EventStoreProjectionManagementClient client, ILogger<ProjectionManager> logger)
        {
            _client = client;
            _logger = logger;
        }
        
        public async void Start()
        {
            const string js = @"
            fromStream('$ce-TodoList').when({
                $init: function(){
                    return {};
                },
                ""EventSourcing.Domain.TodoListCreatedEvent"": function(s,e){
                            s[e.data.AggregateId] = {id: e.data.AggregateId, numberOfTasks: 0, revision: 0};
                },
                ""EventSourcing.Domain.TodoItemAddedEvent"": function(s,e) {
                    s[e.data.AggregateId].numberOfTasks += 1;
                    s[e.data.AggregateId].revision = e.sequenceNumber;
                }
                }).outputState();                  
            ";
            var name = $"TodoList_index";
            try {

                await _client.CreateContinuousAsync(name, js);
            } catch (InvalidOperationException e) when (e.Message.Contains("Conflict")) {
                var format = $"{name} already exists, updating...";
                _logger.LogInformation(format);
                await _client.UpdateAsync(name, js);
                await _client.ResetAsync(name);
            }
        }
    }
    
    public class ListAllTodoListsQueryUsingProjection : IQuery<ListAllTodoListsQueryUsingProjection, JsonElement>
    {
    }

    public class ListAllTodoListsUsingProjectionQueryHandler : IQueryHandler<ListAllTodoListsQueryUsingProjection, JsonElement>
    {
        private readonly EventStoreProjectionManagementClient _client;

        public ListAllTodoListsUsingProjectionQueryHandler(EventStoreProjectionManagementClient client)
        {
            _client = client;
        }
        
        public async Task<JsonElement> Query(ListAllTodoListsQueryUsingProjection query, CancellationToken cancellationToken)
        {
            var name = $"TodoList_index";
            var document = await _client.GetResultAsync(name, cancellationToken: cancellationToken);
            
            return Json(document.RootElement.EnumerateObject()
                .Select(property => property.Value).ToList());
        }
        public static JsonElement Json<T>(T obj)
        {
           return JsonDocument.Parse(JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true })).RootElement;
        }
    }
    
    
    // fromStream('$ce-TodoList').when({
    // $init: function(){
    //     return {};
    // },
    // "EventSourcing.Domain.TodoListCreatedEvent": function(s,e){
    //     s[e.data.AggregateId] = {id: e.data.AggregateId, numberOfTasks: 0, revision: 0};
    // },
    // "EventSourcing.Domain.TodoItemAddedEvent": function(s,e) {
    //     s[e.data.AggregateId].numberOfTasks += 1;
    // }
    // }).outputState();
}