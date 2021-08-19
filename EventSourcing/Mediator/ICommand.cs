using System;

namespace EventSourcing.Mediator
{
    public interface ICommand<TCommand, TResponse>
    where TCommand: ICommand<TCommand, TResponse>
    {
        Guid Id { get; set; }
        long ExpectedRevision { get; set; }
    }

    public abstract class CommandBase<TCommand, TResponse> : ICommand<TCommand, TResponse>
        where TCommand: ICommand<TCommand, TResponse>
    {
        public Guid Id { get; set; }
        public long ExpectedRevision { get; set; } = -1;
    }
}