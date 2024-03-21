using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Domain.Repositories.Abstactions;
public interface IAgentRepository : IBaseRepository<Agent>
{
    Task<IEnumerable<Agent>> GetAvailableAgentsAsync();
    Task<IEnumerable<Agent>> GetAvailableAgentsAsync(ShiftType shiftType);
    Task<Agent> GetNextAvailableAgentAsync();
    Task<IEnumerable<Agent>> GetAgentsBySeniorityAsync(AgentSeniority seniority);
    Task<Agent> AssignChatSessionAsync(Guid agentId, Guid chatSessionId);
    Task<IEnumerable<Agent>> GetOverflowAgentsAsync();
}
