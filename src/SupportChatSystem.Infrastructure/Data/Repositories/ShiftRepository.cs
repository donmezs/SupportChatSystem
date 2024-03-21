using Microsoft.EntityFrameworkCore;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Repositories.Abstactions;
using SupportChatSystem.Infrastructure.Data.Context;

namespace SupportChatSystem.Infrastructure.Data.Repositories;
public class ShiftRepository : IShiftRepository
{
    private readonly ApplicationDbContext _context;

    public ShiftRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Shift> AddAsync(Shift shift)
    {
        await _context.Shifts.AddAsync(shift);
        await _context.SaveChangesAsync();
        return shift;
    }

    public async Task DeleteAsync(Guid shiftId)
    {
        var shift = await _context.Shifts.FindAsync(shiftId);
        if (shift != null)
        {
            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Shift>> GetAllAsync()
    {
        return await _context.Shifts.ToListAsync();
    }

    public async Task<Shift> GetByIdAsync(Guid shiftId)
    {
        return await _context.Shifts.FindAsync(shiftId);
    }

    public async Task<Shift> UpdateAsync(Shift shift)
    {
        _context.Shifts.Update(shift);
        await _context.SaveChangesAsync();
        return shift;
    }

    public async Task<IEnumerable<Shift>> GetShiftsByTypeAsync(ShiftType shiftType)
    {
        return await _context.Shifts
                             .Where(s => s.ShiftType == shiftType)
                             .ToListAsync();
    }

    public async Task<IEnumerable<Shift>> GetShiftsForAgentAsync(Guid agentId)
    {
        return await _context.Shifts
                             .Where(s => s.Agents.Any(a => a.Id == agentId))
                             .ToListAsync();
    }

    public async Task AssignAgentToShiftAsync(Guid agentId, Guid shiftId)
    {
        var shift = await _context.Shifts.Include(s => s.Agents).FirstOrDefaultAsync(s => s.Id == shiftId);
        var agent = await _context.Agents.FindAsync(agentId);
        if (shift != null && agent != null && !shift.Agents.Contains(agent))
        {
            shift.Agents.Add(agent);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Agent>> GetAgentsByShiftAsync(Guid shiftId)
    {
        var shift = await _context.Shifts.Include(s => s.Agents).FirstOrDefaultAsync(s => s.Id == shiftId);
        return shift?.Agents ?? new List<Agent>();
    }
}
