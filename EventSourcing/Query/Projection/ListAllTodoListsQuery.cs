using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Mediator;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Query.Projection
{
    public class TodoListIndexProjectionManager : AbstractProjectionManager
    {
        public TodoListIndexProjectionManager(EventStoreProjectionManagementClient client, ILogger<TodoListIndexProjectionManager> logger) :
            base(client, logger, "TodoList_index",
                @"
                    fromStream('$ce-TodoList').when({
                        $init: function(){
                            return {};
                        },
                        ""EventSourcing.Domain.TodoListCreatedEvent"": function(s,e){
                                    s[e.data.AggregateId] = {id: e.data.AggregateId, numberOfTasks: 0, revision: 0};
                        },
                        ""EventSourcing.Domain.TodoItemAddedEvent"": function(s,e) {
                            s[e.data.AggregateId].numberOfTasks += 1;
                            s[e.data.AggregateId].revision =  +e.sequenceNumber;
                        }
                    }).outputState();         
            ")
        {
        }
    }

    public class ListAllTodoListsUsingProjectionQuery : IQuery<ListAllTodoListsUsingProjectionQuery, JsonElement>
    {
    }

    public class
        ListAllTodoListsUsingProjectionQueryHandler : IQueryHandler<ListAllTodoListsUsingProjectionQuery, JsonElement>
    {
        private readonly EventStoreProjectionManagementClient _client;

        public ListAllTodoListsUsingProjectionQueryHandler(EventStoreProjectionManagementClient client)
        {
            _client = client;
        }

        public async Task<JsonElement> Query(ListAllTodoListsUsingProjectionQuery query,
            CancellationToken cancellationToken)
        {
            var document = await _client.GetResultAsync($"TodoList_index", cancellationToken: cancellationToken);
        
            return Json(document.RootElement.EnumerateObject()
                .Select(property => property.Value).ToList());
        }

        private static JsonElement Json<T>(T obj)
        {
            return JsonDocument.Parse(JsonSerializer.Serialize(obj, new JsonSerializerOptions {WriteIndented = true}))
                .RootElement;
        }
    }
}