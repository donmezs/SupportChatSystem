using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Domain.Events;
public class ChatSessionStatusChangedEvent : DomainEvent
{
    public ChatSession ChatSession { get; }
    public ChatSessionStatus PreviousStatus { get; }
    public ChatSessionStatus NewStatus { get; }

    public ChatSessionStatusChangedEvent(ChatSession chatSession, ChatSessionStatus previousStatus, ChatSessionStatus newStatus)
    {
        ChatSession = chatSession;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
    }
}

