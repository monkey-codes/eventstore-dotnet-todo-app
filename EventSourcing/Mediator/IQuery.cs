namespace EventSourcing.Mediator
{
    public interface IQuery<TQuery, TResponse>
    where TQuery: IQuery<TQuery, TResponse>
    {
        
    }
}