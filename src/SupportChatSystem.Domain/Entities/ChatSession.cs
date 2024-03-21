using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Domain.Entities;
public class ChatSession : BaseEntity
{
    public ChatSessionStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? LastPolledTime { get; set; }
    public Guid AgentId { get; set; }

    // Navigation property for EF Core relationship
    public virtual Agent Agent { get; set; }

    public ChatSession()
    {
    }

    public ChatSession(Guid agentId)
    {
        AgentId = agentId;
        StartTime = DateTime.UtcNow;
        Status = ChatSessionStatus.Waiting;
    }
}
