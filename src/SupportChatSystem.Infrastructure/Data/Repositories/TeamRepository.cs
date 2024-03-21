using Microsoft.EntityFrameworkCore;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Repositories.Abstactions;
using SupportChatSystem.Infrastructure.Data.Context;

namespace SupportChatSystem.Infrastructure.Data.Repositories;
public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;

    public TeamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Team> AddAsync(Team team)
    {
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        return team;
    }

    public async Task DeleteAsync(Guid teamId)
    {
        var team = await _context.Teams.FindAsync(teamId);
        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _context.Teams.ToListAsync();
    }

    public async Task<Team> GetByIdAsync(Guid teamId)
    {
        return await _context.Teams
                             .Include(t => t.Agents)
                             .FirstOrDefaultAsync(t => t.Id == teamId);
    }

    public async Task<Team> UpdateAsync(Team team)
    {
        _context.Teams.Update(team);
        await _context.SaveChangesAsync();
        return team;
    }

    public async Task<IEnumerable<Team>> GetTeamsWithAvailableCapacityAsync()
    {
        var teamsWithAvailableCapacity = new List<Team>();

        var teams = await _context.Teams.Include(t => t.Agents).ToListAsync();
        foreach (var team in teams)
        {
            int teamCapacity = await CalculateTeamCapacityAsync(team.Id);
            int activeSessionsCount = await GetActiveChatSessionsForTeamCountAsync(team.Id);

            // Check if the current number of active chat sessions is less than the team's capacity
            if (activeSessionsCount < teamCapacity)
            {
                teamsWithAvailableCapacity.Add(team);
            }
        }

        return teamsWithAvailableCapacity;
    }

    public async Task<int> CalculateTeamCapacityAsync(Guid teamId)
    {
        var team = await GetByIdAsync(teamId);
        if (team == null) return 0;

        // Example capacity calculation based on agent seniority and predefined multipliers
        int capacity = team.Agents.Sum(agent =>
        {
            switch (agent.Seniority)
            {
                case AgentSeniority.Junior: return (int)(10 * 0.4);
                case AgentSeniority.MidLevel: return (int)(10 * 0.6);
                case AgentSeniority.Senior: return (int)(10 * 0.8);
                case AgentSeniority.TeamLead: return (int)(10 * 0.5);
                default: return 0;
            }
        });

        return capacity;
    }

    private async Task<int> GetActiveChatSessionsForTeamCountAsync(Guid teamId)
    {
        return await _context
            .ChatSessions
            .CountAsync(cs => cs.Agent.TeamId == teamId
                           && cs.Status == ChatSessionStatus.Active);
    }

    public async Task<IEnumerable<Agent>> GetAgentsByTeamAsync(Guid teamId)
    {
        var team = await _context.Teams
                                 .Include(t => t.Agents)
                                 .FirstOrDefaultAsync(t => t.Id == teamId);
        return team?.Agents ?? new List<Agent>();
    }

    public async Task AssignAgentToTeamAsync(Guid agentId, Guid teamId)
    {
        var team = await _context.Teams.Include(t => t.Agents).FirstOrDefaultAsync(t => t.Id == teamId);
        var agent = await _context.Agents.FindAsync(agentId);
        if (team != null && agent != null && !team.Agents.Contains(agent))
        {
            team.Agents.Add(agent);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ChatSession>> GetActiveChatSessionsForTeamAsync(Guid teamId)
    {
        // This method assumes a direct relationship between Teams and ChatSessions or indirect via Agents
        // Adjust the query according to your data model
        var teamChatSessions = await _context.ChatSessions
                                             .Where(cs => cs.Agent.TeamId == teamId && cs.Status == ChatSessionStatus.Active)
                                             .ToListAsync();
        return teamChatSessions;
    }

    public async Task<Team> GetTeamWithMembersAsync(Guid teamId)
    {
        return await _context.Teams
                             .Include(t => t.Agents)
                             .FirstOrDefaultAsync(t => t.Id == teamId);
    }

    public async Task<IEnumerable<Team>> GetAllTeamsAsync()
    {
        return await GetAllAsync();
    }

    public async Task<Team> GetOverflowTeamAsync()
    {
        // Assuming there's a boolean property `IsOverflow` to identify the overflow team
        return await _context.Teams
                             .Include(team => team.Agents) // Assuming you want to include team members
                             .FirstOrDefaultAsync(team => team.IsOverflow);
    }
}
