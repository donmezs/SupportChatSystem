using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Domain.Repositories.Abstactions;
public interface IChatSessionRepository : IBaseRepository<ChatSession>
{
    Task<IEnumerable<ChatSession>> GetSessionsByStatusAsync(ChatSessionStatus status);
    Task<int> GetActiveAndWaitingChatSessionsCountAsync();
    Task<IEnumerable<ChatSession>> GetActiveChatSessionsAsync();
    Task<IEnumerable<ChatSession>> GetActiveSessionsForAgentAsync(Guid agentId);
    Task<ChatSession> MarkSessionAsInactiveAsync(Guid sessionId);
    Task<IEnumerable<ChatSession>> GetWaitingSessionsFIFOAsync();
    Task<ChatSession> GetNextWaitingSessionAsync();
}
