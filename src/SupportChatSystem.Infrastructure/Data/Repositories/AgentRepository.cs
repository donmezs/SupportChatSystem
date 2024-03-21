using Microsoft.EntityFrameworkCore;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Repositories.Abstactions;
using SupportChatSystem.Infrastructure.Data.Context;

namespace SupportChatSystem.Infrastructure.Data.Repositories;
public class AgentRepository : IAgentRepository
{
    private readonly ApplicationDbContext _context;
    public AgentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Agent> GetByIdAsync(Guid id)
    {
        return await _context.Agents
                             .Include(agent => agent.Shift) // Include related Shift
                             .Include(agent => agent.ChatSessions) // Include related ChatSessions
                             .FirstOrDefaultAsync(agent => agent.Id == id);
    }

    public async Task<IEnumerable<Agent>> GetAllAsync()
    {
        return await _context.Agents
                         .Include(a => a.Shift) // Eagerly load the Shift navigation property
                         .Include(a => a.Team) // Optionally, include this to load the Team as well
                         .ToListAsync();
    }

    public async Task<Agent> AddAsync(Agent entity)
    {
        await _context.Agents.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Agent> UpdateAsync(Agent entity)
    {
        _context.Agents.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Agents.FindAsync(id);
        if (entity != null)
        {
            _context.Agents.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Agent>> GetAvailableAgentsAsync(ShiftType shiftType)
    {
        var now = DateTime.UtcNow;
        return await _context.Agents
                             .Include(a => a.Shift)
                             .Where(a => a.Shift.ShiftType == shiftType && a.Shift.StartTime <= now && a.Shift.EndTime > now)
                             .ToListAsync();
    }

    public async Task<IEnumerable<Agent>> GetAvailableAgentsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Agents
                             .Include(a => a.Shift)
                             .Where(a => a.Shift.StartTime <= now && a.Shift.EndTime > now)
                             .ToListAsync();
    }

    public async Task<Agent> GetNextAvailableAgentAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Agents
                             .Include(a => a.Shift)
                             .OrderBy(a => Guid.NewGuid()) // Random selection for demonstration
                             .FirstOrDefaultAsync(a => a.Shift.StartTime <= now && a.Shift.EndTime > now && a.ChatSessions.Count < 10);
    }

    public async Task<IEnumerable<Agent>> GetAgentsBySeniorityAsync(AgentSeniority seniority)
    {
        return await _context.Agents
                             .Where(a => a.Seniority == seniority)
                             .ToListAsync();
    }

    public async Task<Agent> AssignChatSessionAsync(Guid agentId, Guid chatSessionId)
    {
        var agent = await GetByIdAsync(agentId); // Utilizing base method
        var chatSession = await _context.ChatSessions.FindAsync(chatSessionId);
        if (agent != null && chatSession != null)
        {
            // Assuming Agent entity has a method to assign a chat session
            agent.ChatSessions.Add(chatSession); // Simplified; adjust based on your domain model
            await UpdateAsync(agent); // Utilizing base method
        }
        return agent;
    }

    public async Task<IEnumerable<Agent>> GetOverflowAgentsAsync()
    {
        // Adjust based on your criteria for identifying overflow agents
        return await _context.Agents
                             .Where(a => a.IsOverflow) // Assuming an IsOverflow property or similar logic
                             .ToListAsync();
    }
}
