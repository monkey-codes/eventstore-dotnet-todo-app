namespace EventSourcing.Mediator
{
    public interface IHello
    {
        public string SayHello(string name);
        
    }
    public class Hello: IHello
    {
        public string SayHello(string name)
        {
            return $"Hello {name}";
        }
    }
}