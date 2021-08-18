namespace EventSourcing.Mediator
{
    public interface ICommand<TCommand, TResponse>
    where TCommand: ICommand<TCommand, TResponse>
    {
        
    }
}