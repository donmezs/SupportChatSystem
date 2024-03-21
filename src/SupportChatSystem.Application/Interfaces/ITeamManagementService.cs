namespace SupportChatSystem.Application.Interfaces;
public interface ITeamManagementService
{
    Task<int> CalculateTeamCapacityAsync(Guid teamId);
    Task<bool> SetOverflowTeamAsync(Guid teamId);
    Task<bool> UnsetOverflowTeamAsync();
}
