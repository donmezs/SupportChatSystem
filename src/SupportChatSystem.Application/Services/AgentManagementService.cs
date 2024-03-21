using SupportChatSystem.Application.Interfaces;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Exceptions;
using SupportChatSystem.Domain.Repositories.Abstactions;

namespace SupportChatSystem.Application.Services;
public class AgentManagementService : IAgentManagementService
{
    private static int lastAssignedAgentIndex = -1; // Maintain state for round-robin distribution

    private readonly IAgentRepository _agentRepository;

    public AgentManagementService(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public bool IsAgentAvailable(Agent agent)
    {
        if (agent == null)
        {
            return false;
        }

        var nowUtc = DateTime.UtcNow;

        return IsAgentCurrentlyInShift(agent, nowUtc) && HasAgentCapacityForNewChatSession(agent);
    }

    public async Task<Agent> GetNextAvailableAgent(DateTime currentTime)
    {
        var agents = await _agentRepository.GetAllAsync();
        var orderedAgents = agents.OrderBy(a => a.Seniority).ThenBy(a => a.Id).ToList(); // Consistent ordering

        var startingIndex = (lastAssignedAgentIndex + 1) % orderedAgents.Count;
        var index = startingIndex;

        do
        {
            var agent = orderedAgents[index];
            if (IsAgentCurrentlyInShift(agent, currentTime) && HasAgentCapacityForNewChatSession(agent))
            {
                lastAssignedAgentIndex = index; // Update for next round-robin selection
                return agent;
            }

            index = (index + 1) % orderedAgents.Count; // Round-robin advancement
        } while (index != startingIndex); // Loop once through all agents

        return null; // If no available agent is found
    }

    public bool IsAgentCurrentlyInShift(Agent agent, DateTime currentTime)
    {
        return currentTime >= agent.Shift.StartTime && currentTime < agent.Shift.EndTime;
    }

    public bool HasAgentCapacityForNewChatSession(Agent agent, int additionalSessions = 0)
    {
        var currentChatSessionCount = agent.ChatSessions.Count(cs => cs.Status == ChatSessionStatus.Active) + additionalSessions;
        var maxConcurrentChats = CalculateAgentCapacity(agent);

        return currentChatSessionCount < maxConcurrentChats;
    }

    public int CalculateAgentCapacity(Agent agent)
    {
        const int baseMaxChats = 10;
        decimal efficiencyMultiplier = GetEfficiencyMultiplier(agent.Seniority);

        return (int)Math.Floor(baseMaxChats * efficiencyMultiplier);
    }

    private decimal GetEfficiencyMultiplier(AgentSeniority seniority)
    {
        return seniority switch
        {
            AgentSeniority.Junior => 0.4m,
            AgentSeniority.MidLevel => 0.6m,
            AgentSeniority.Senior => 0.8m,
            AgentSeniority.TeamLead => 0.5m,
            _ => throw new UnknownSeniorityLevelException("Unknown agent seniority level."),
        };
    }

    public bool IsWithinOfficeHours()
    {
        // Define office hours start and end times
        var startOfficeHours = new TimeSpan(9, 0, 0); // 9 AM
        var endOfficeHours = new TimeSpan(17, 0, 0); // 5 PM
        var now = DateTime.UtcNow.TimeOfDay;

        return now >= startOfficeHours && now <= endOfficeHours;
    }

    public async Task<(int totalCapacity, int maximumQueueLength)> CalculateCapacityAndMaxQueueLengthAsync()
    {
        var agents = await _agentRepository.GetAllAsync();
        int totalCapacity = agents.Sum(agent => CalculateAgentCapacity(agent));
        int maximumQueueLength = (int)(totalCapacity * 1.5);

        return (totalCapacity, maximumQueueLength);
    }
}
