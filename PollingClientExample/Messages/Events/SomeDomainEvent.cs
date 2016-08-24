namespace Messages.Events
{
    public class SomeDomainEvent
    {
        public SomeDomainEvent(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}