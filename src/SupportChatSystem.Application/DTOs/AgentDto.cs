using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Application.DTOs;
public class AgentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsOverflow { get;set; }
    public AgentSeniority Seniority { get; set; }
    public Guid ShiftId { get; set; }
    public Guid TeamId { get; set; }
}

