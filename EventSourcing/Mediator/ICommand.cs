namespace EventSourcing.Mediator
{
    public interface ICommand<TCommand, TResponse>
    where TCommand: ICommand<TCommand, TResponse>
    {
        long ExpectedRevision { get; set; }
    }

    public abstract class CommandBase<TCommand, TResponse> : ICommand<TCommand, TResponse>
        where TCommand: ICommand<TCommand, TResponse>
    {
        public long ExpectedRevision { get; set; } = -1;
    }
}