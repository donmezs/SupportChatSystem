using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Domain.Repositories.Abstactions;
public interface ITeamRepository : IBaseRepository<Team>
{
    Task<IEnumerable<Team>> GetTeamsWithAvailableCapacityAsync();
    Task<int> CalculateTeamCapacityAsync(Guid teamId);
    Task<IEnumerable<Agent>> GetAgentsByTeamAsync(Guid teamId);
    Task AssignAgentToTeamAsync(Guid agentId, Guid teamId);
    Task<IEnumerable<ChatSession>> GetActiveChatSessionsForTeamAsync(Guid teamId);
    Task<IEnumerable<Team>> GetAllTeamsAsync();
    Task<Team> GetTeamWithMembersAsync(Guid teamId);
    Task<Team> GetOverflowTeamAsync();
}
