using SupportChatSystem.Application.Interfaces;
using SupportChatSystem.Domain.Repositories.Abstactions;

namespace SupportChatSystem.Application.Services;
public class TeamManagementService : ITeamManagementService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IAgentManagementService _agentManagementService;
    public TeamManagementService(ITeamRepository teamRepository, IAgentManagementService agentManagementService)
    {
        _teamRepository = teamRepository;
        _agentManagementService = agentManagementService;

    }

    public async Task<int> CalculateTeamCapacityAsync(Guid teamId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null) throw new ArgumentException("Team not found.", nameof(teamId));

        return team.Agents.Sum(agent => _agentManagementService.CalculateAgentCapacity(agent));
    }

    public async Task<bool> SetOverflowTeamAsync(Guid teamId)
    {
        // First, ensure any currently set overflow team is unset
        var currentOverflowTeam = await _teamRepository.GetOverflowTeamAsync();
        if (currentOverflowTeam != null)
        {
            currentOverflowTeam.IsOverflow = false;
            await _teamRepository.UpdateAsync(currentOverflowTeam);
        }

        // Set the new overflow team
        var teamToSetAsOverflow = await _teamRepository.GetByIdAsync(teamId);
        if (teamToSetAsOverflow == null) return false;

        teamToSetAsOverflow.IsOverflow = true;
        await _teamRepository.UpdateAsync(teamToSetAsOverflow);
        return true;
    }

    public async Task<bool> UnsetOverflowTeamAsync()
    {
        var currentOverflowTeam = await _teamRepository.GetOverflowTeamAsync();
        if (currentOverflowTeam == null) return false;

        currentOverflowTeam.IsOverflow = false;
        await _teamRepository.UpdateAsync(currentOverflowTeam);
        return true;
    }
}
