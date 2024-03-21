using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Domain.Events;
public class AgentShiftAssignmentChangedEvent : DomainEvent
{
    public Agent Agent { get; }
    public Shift Shift { get; }
    public bool IsAssigned { get; }

    public AgentShiftAssignmentChangedEvent(Agent agent, Shift shift, bool isAssigned)
    {
        Agent = agent;
        Shift = shift;
        IsAssigned = isAssigned;
    }
}
