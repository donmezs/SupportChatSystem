namespace SupportChatSystem.Application.DTOs;
public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<AgentDto> Agents { get; set; } = new List<AgentDto>();
}

