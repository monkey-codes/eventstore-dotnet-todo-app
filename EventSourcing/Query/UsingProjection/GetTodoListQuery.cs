using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Mediator;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Query.UsingProjection
{
    public class GetTodoListProjectionManager : AbstractProjectionManager
    {
        public GetTodoListProjectionManager(EventStoreProjectionManagementClient client,
            ILogger<TodoListIndexProjectionManager> logger) :
            base(client, logger, "TodoList_get",
                @"
                    fromCategory('TodoList')
                    .foreachStream()
                    .when( { 
                        $init: function(){
                            return {id: '', tasks: [], revision: 0};
                        },
                        'EventSourcing.Domain.TodoListCreatedEvent': function(s,e) {
                            s.id = e.data.AggregateId;  
                        },
                        'EventSourcing.Domain.TodoItemAddedEvent': function(s,e) {
                            s.tasks.push({taskId: e.data.ItemId, description: e.data.Description });
                            s.revision = +e.sequenceNumber;
                        }
                    }).outputState();  
            ")
        {
        }
    }

    public class GetTodoListUsingProjectionQuery : IQuery<GetTodoListUsingProjectionQuery, JsonElement>
    {
        public Guid Id { get; set; }
    }


    public class
        GetTodoListUsingProjectionQueryHandler : IQueryHandler<GetTodoListUsingProjectionQuery, JsonElement>
    {
        private readonly EventStoreProjectionManagementClient _client;

        public GetTodoListUsingProjectionQueryHandler(EventStoreProjectionManagementClient client)
        {
            _client = client;
        }

        public async Task<JsonElement> Query(GetTodoListUsingProjectionQuery query, CancellationToken cancellationToken)
        {
            var document = await _client.GetResultAsync($"TodoList_get",
                cancellationToken: cancellationToken, partition: $"TodoList-{query.Id}");
            return document.RootElement;
        }
    }
}