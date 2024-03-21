using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Application.DTOs;
public class ShiftDto
{
    public Guid Id { get; set; }
    public ShiftType ShiftType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

