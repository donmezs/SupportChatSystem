using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Application.Interfaces;
public interface IChatSessionManagementService
{
    Task<ChatSession> CreateChatSessionAsync();
    Task<(bool IsAssigned, Guid? AssignedAgentId)> AssignChatSessionAsync(Guid chatSessionId);
    Task<bool> MarkInactiveSessionAsync(Guid chatSessionId);
    Task<bool> SetChatSessionStatus(Guid chatSessionId, ChatSessionStatus status);
}
