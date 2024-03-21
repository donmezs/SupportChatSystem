using SupportChatSystem.Application.Interfaces;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Repositories.Abstactions;

namespace SupportChatSystem.Application.Services;
public class ShiftManagementService : IShiftManagementService
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IAgentRepository _agentRepository;

    public ShiftManagementService(IShiftRepository shiftRepository, IAgentRepository agentRepository)
    {
        _shiftRepository = shiftRepository;
        _agentRepository = agentRepository;
    }

    public async Task<bool> AssignAgentToShiftAsync(Guid agentId, Guid shiftId)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        if (agent == null) throw new ArgumentException("Agent not found.", nameof(agentId));

        var shift = await _shiftRepository.GetByIdAsync(shiftId);
        if (shift == null) throw new ArgumentException("Shift not found.", nameof(shiftId));

        // Prevent reassignment if the agent is already assigned to this shift
        if (agent.ShiftId == shiftId) return false;

        // Update the agent's shift assignment
        agent.ShiftId = shiftId;
        await _agentRepository.UpdateAsync(agent);

        return true;
    }

    public async Task<IEnumerable<Agent>> GetAgentsInShiftAsync(Guid shiftId)
    {
        var shift = await _shiftRepository.GetByIdAsync(shiftId);
        if (shift == null) throw new ArgumentException("Shift not found.", nameof(shiftId));

        // Assuming GetAgentsByShiftAsync is properly implemented in the IShiftRepository
        var agentsInShift = await _shiftRepository.GetAgentsByShiftAsync(shiftId);
        return agentsInShift;
    }

    public async Task<IEnumerable<Shift>> GetShiftsForAgentAsync(Guid agentId)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        if (agent == null) throw new ArgumentException("Agent not found.", nameof(agentId));

        // Assuming the agent might be associated with multiple shifts, and there's a way to retrieve them.
        // This requires support in the IShiftRepository interface and its implementations.
        var shiftsForAgent = await _shiftRepository.GetShiftsForAgentAsync(agentId);
        return shiftsForAgent;
    }
}
