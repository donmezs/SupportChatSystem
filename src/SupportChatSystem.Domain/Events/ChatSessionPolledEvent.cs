namespace SupportChatSystem.Domain.Events;
public class ChatSessionPolledEvent : DomainEvent
{
    public Guid ChatSessionId { get; }
    public DateTime LastPolledTime { get; }

    public ChatSessionPolledEvent(Guid chatSessionId, DateTime lastPolledTime)
    {
        ChatSessionId = chatSessionId;
        LastPolledTime = lastPolledTime;
    }
}
