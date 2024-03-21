using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Application.DTOs;
public class ChatSessionDto
{
    public Guid Id { get; set; }
    public ChatSessionStatus Status { get; set; }
    public DateTime StartTime { get; set; }
}

