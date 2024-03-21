namespace SupportChatSystem.Domain.Events;
public abstract class DomainEvent
{
    protected DomainEvent() => OccurredOn = DateTime.UtcNow;
    public DateTime OccurredOn { get; }
}
