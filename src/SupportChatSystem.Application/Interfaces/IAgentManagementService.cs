using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Application.Interfaces;
public interface IAgentManagementService
{
    bool IsAgentCurrentlyInShift(Agent agent, DateTime currentTime);
    bool HasAgentCapacityForNewChatSession(Agent agent, int additionalSessions = 0);
    bool IsAgentAvailable(Agent agent);
    bool IsWithinOfficeHours();
    int CalculateAgentCapacity(Agent agent);
    Task<Agent> GetNextAvailableAgent(DateTime currentTime);
    Task<(int totalCapacity, int maximumQueueLength)> CalculateCapacityAndMaxQueueLengthAsync();
}
