using SupportChatSystem.Domain.Entities;

namespace SupportChatSystem.Application.Interfaces;
public interface IShiftManagementService
{
    Task<bool> AssignAgentToShiftAsync(Guid agentId, Guid shiftId);
    Task<IEnumerable<Agent>> GetAgentsInShiftAsync(Guid shiftId);
    Task<IEnumerable<Shift>> GetShiftsForAgentAsync(Guid agentId);
}
