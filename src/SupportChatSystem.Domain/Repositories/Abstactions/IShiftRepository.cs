using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Domain.Repositories.Abstactions;
public interface IShiftRepository : IBaseRepository<Shift>
{
    Task<IEnumerable<Shift>> GetShiftsByTypeAsync(ShiftType shiftType);
    Task<IEnumerable<Shift>> GetShiftsForAgentAsync(Guid agentId);
    Task AssignAgentToShiftAsync(Guid agentId, Guid shiftId);
    Task<IEnumerable<Agent>> GetAgentsByShiftAsync(Guid shiftId);
}
