using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Domain.Events;
public class OverflowStatusChangedEvent : DomainEvent
{
    public OverflowStatus PreviousStatus { get; }
    public OverflowStatus NewStatus { get; }

    public OverflowStatusChangedEvent(OverflowStatus previousStatus, OverflowStatus newStatus)
    {
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
    }
}
