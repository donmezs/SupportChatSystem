using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Domain.Events;
public class TeamCapacityChangedEvent : DomainEvent
{
    public Team Team { get; }
    public int PreviousCapacity { get; }
    public int NewCapacity { get; }

    public TeamCapacityChangedEvent(Team team, int previousCapacity, int newCapacity)
    {
        Team = team;
        PreviousCapacity = previousCapacity;
        NewCapacity = newCapacity;
    }
}
