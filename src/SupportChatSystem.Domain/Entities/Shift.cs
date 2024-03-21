using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Exceptions;

namespace SupportChatSystem.Domain.Entities;
public class Shift : BaseEntity
{
    public ShiftType ShiftType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // Navigation property for EF Core
    public virtual ICollection<Agent> Agents { get; set; } = new List<Agent>();

    public Shift(ShiftType shiftType, DateTime startTime, DateTime endTime)
    {
        if (endTime <= startTime)
            throw new ShiftTimeInvalidException("End time must be after start time.");

        ShiftType = shiftType;
        StartTime = startTime;
        EndTime = endTime;
    }
}