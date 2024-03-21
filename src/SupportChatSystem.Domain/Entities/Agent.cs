using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Exceptions;

namespace SupportChatSystem.Domain.Entities;
public class Agent : BaseEntity
{
    public string Name { get; set; }
    public AgentSeniority Seniority { get; set; }
    public bool IsOverflow { get; set; }
    public Guid ShiftId { get; set; }
    public Guid TeamId { get; set; }

    // Navigation properties for EF Core
    public virtual Shift Shift { get; set; }
    public virtual Team Team { get; set; }
    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();

    public Agent(string name, AgentSeniority seniority, Guid shiftId, Guid teamId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new AgentNameInvalidException("Agent name cannot be null or whitespace.");

        Name = name;
        Seniority = seniority;
        ShiftId = shiftId;
        TeamId = teamId;
    }
}

