using EventSourcing.EventSourcing;

namespace EventSourcing.Domain
{
    public class AddTodoItemCommandHandler : GenericCommandHandler<AddTodoItemCommand, TodoList, object>
    {
        public AddTodoItemCommandHandler(IEventStoreRepository<TodoList> repository) : base(repository)
        {
        }
    }
    
    public class CreateTodoListCommandHandler: GenericCommandHandler<CreateTodoListCommand, TodoList, object>
    {
        public CreateTodoListCommandHandler(IEventStoreRepository<TodoList> repository) : base(repository)
        {
        }
    }
    
}