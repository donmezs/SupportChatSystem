using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Domain.Events;
public class ChatSessionAssignedEvent : DomainEvent
{
    public ChatSession ChatSession { get; }
    public Agent AssignedAgent { get; }

    public ChatSessionAssignedEvent(ChatSession chatSession, Agent assignedAgent)
    {
        ChatSession = chatSession;
        AssignedAgent = assignedAgent;
    }
}
