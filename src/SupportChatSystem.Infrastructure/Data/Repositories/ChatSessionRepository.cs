using Microsoft.EntityFrameworkCore;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Repositories.Abstactions;
using SupportChatSystem.Infrastructure.Data.Context;

namespace SupportChatSystem.Infrastructure.Data.Repositories;
public class ChatSessionRepository : IChatSessionRepository
{
    private readonly ApplicationDbContext _context;

    public ChatSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChatSession> AddAsync(ChatSession session)
    {
        var createdSession = await _context.ChatSessions.AddAsync(session);
        await _context.SaveChangesAsync();
        return createdSession.Entity;
    }

    public async Task DeleteAsync(Guid sessionId)
    {
        var session = await _context.ChatSessions.FindAsync(sessionId);
        if (session != null)
        {
            _context.ChatSessions.Remove(session);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ChatSession>> GetAllAsync()
    {
        return await _context.ChatSessions.Include(cs => cs.Agent).ToListAsync();
    }

    public async Task<ChatSession> GetByIdAsync(Guid sessionId)
    {

        return _context.ChatSessions
            .Where(cs => cs.Id == sessionId)
            .FirstOrDefault();
    }

    public async Task<ChatSession> UpdateAsync(ChatSession session)
    {
        _context.ChatSessions.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<IEnumerable<ChatSession>> GetSessionsByStatusAsync(ChatSessionStatus status)
    {
        return await _context.ChatSessions
                             .Where(cs => cs.Status == status)
                             .ToListAsync();
    }

    public async Task<int> GetActiveAndWaitingChatSessionsCountAsync()
    {
        return await _context.ChatSessions
                             .CountAsync(cs => cs.Status == ChatSessionStatus.Active || cs.Status == ChatSessionStatus.Waiting);
    }

    public async Task<IEnumerable<ChatSession>> GetActiveChatSessionsAsync()
    {
        return await GetSessionsByStatusAsync(ChatSessionStatus.Active);
    }

    public async Task<IEnumerable<ChatSession>> GetActiveSessionsForAgentAsync(Guid agentId)
    {
        return await _context.ChatSessions
                             .Where(cs => cs.AgentId == agentId && cs.Status == ChatSessionStatus.Active)
                             .ToListAsync();
    }

    public async Task<ChatSession> MarkSessionAsInactiveAsync(Guid sessionId)
    {
        var session = await _context.ChatSessions.FindAsync(sessionId);
        if (session != null)
        {
            session.Status = ChatSessionStatus.Inactive;
            await _context.SaveChangesAsync();
        }
        return session;
    }

    public async Task<IEnumerable<ChatSession>> GetWaitingSessionsFIFOAsync()
    {
        return await _context.ChatSessions
                             .Where(cs => cs.Status == ChatSessionStatus.Waiting)
                             .OrderBy(cs => cs.StartTime)
                             .ToListAsync();
    }

    public async Task<ChatSession> GetNextWaitingSessionAsync()
    {
        return await _context.ChatSessions
                             .Where(cs => cs.Status == ChatSessionStatus.Waiting)
                             .OrderBy(cs => cs.StartTime)
                             .FirstOrDefaultAsync();
    }
}
