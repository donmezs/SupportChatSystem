using Moq;
using SupportChatSystem.Application.Services;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Repositories.Abstactions;

namespace SupportChatSystem.Tests.Services;
public class AgentManagementServiceTests
{
    private readonly Mock<IAgentRepository> _agentRepositoryMock = new Mock<IAgentRepository>();
    private AgentManagementService CreateService() => new AgentManagementService(_agentRepositoryMock.Object);

    [Fact]
    public async Task GetNextAvailableAgent_ReturnsAgentInRoundRobinOrder()
    {
        // Arrange
        var currentTime = DateTime.UtcNow;
        var shiftStartTime = currentTime.AddHours(-1);
        var shiftEndTime = currentTime.AddHours(1);
        var shift = new Shift(ShiftType.Morning, shiftStartTime, shiftEndTime);
        var team = new Team("Rocket Team");

        var agents = new List<Agent>
        {
            CreateAgent("Agent 1", AgentSeniority.Junior, shift, team),
            CreateAgent("Agent 2", AgentSeniority.Senior, shift, team)
        };

        _agentRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(agents);

        var service = CreateService();

        // Act
        var firstAgent = await service.GetNextAvailableAgent(DateTime.UtcNow);
        var secondAgent = await service.GetNextAvailableAgent(DateTime.UtcNow);
        var thirdAgent = await service.GetNextAvailableAgent(DateTime.UtcNow);

        // Assert
        Assert.Equal(agents[0].Id, firstAgent.Id);
        Assert.Equal(agents[1].Id, secondAgent.Id);
        Assert.Equal(agents[0].Id, thirdAgent.Id); // Should loop back to the first agent in round-robin order
    }

    [Fact]
    public void IsAgentAvailable_ReturnsTrue_WhenAgentMeetsConditions()
    {
        // Arrange
        var currentTime = DateTime.UtcNow;
        var shiftStartTime = currentTime.AddHours(-1);
        var shiftEndTime = currentTime.AddHours(1);
        var shift = CreateShift(ShiftType.Morning);
        var team = new Team("Rocket Team");

        var agent = CreateAgent("John Doe", AgentSeniority.Junior, shift, team);

        var service = CreateService();

        // Act
        var result = service.IsAgentAvailable(agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CalculateAgentCapacity_ReturnsCorrectCapacityBasedOnSeniority()
    {
        // Arrange
        var shift = CreateShift(ShiftType.Morning);
        var team = new Team("Rocket Team");
        var juniorAgent = CreateAgent("Agent 1", AgentSeniority.Junior, shift, team);
        var seniorAgent = CreateAgent("Agent 2", AgentSeniority.Senior, shift, team);

        var service = CreateService();

        // Act
        var juniorCapacity = service.CalculateAgentCapacity(juniorAgent);
        var seniorCapacity = service.CalculateAgentCapacity(seniorAgent);

        // Assert
        Assert.Equal(4, juniorCapacity); // 10 * 0.4
        Assert.Equal(8, seniorCapacity); // 10 * 0.8
    }

    private Shift CreateShift(ShiftType shiftType)
    {
        var currentTime = DateTime.UtcNow;
        var shiftStartTime = currentTime.AddHours(-1);
        var shiftEndTime = currentTime.AddHours(1);
        var shift = new Shift(shiftType, shiftStartTime, shiftEndTime);

        return shift;
    }

    private Agent CreateAgent(string name, AgentSeniority seniority, Shift shift, Team team)
    {
        var agent = new Agent(name, seniority, shift.Id, team.Id);
        agent.Shift = shift;
        agent.Team = team;

        return agent;
    }
}
