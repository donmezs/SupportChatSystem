using Microsoft.Extensions.Logging;
using Moq;
using SupportChatSystem.Application.Interfaces;
using SupportChatSystem.Application.Services;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Repositories.Abstactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportChatSystem.Tests.Services;
public class ChatSessionManagementServiceTests
{
    private readonly Mock<IChatSessionRepository> _chatSessionRepositoryMock = new Mock<IChatSessionRepository>();
    private readonly Mock<ITeamRepository> _teamRepositoryMock = new Mock<ITeamRepository>();
    private readonly Mock<IAgentManagementService> _agentManagementServiceMock = new Mock<IAgentManagementService>();
    private readonly Mock<ILogger<ChatSessionManagementService>> _loggerMock = new Mock<ILogger<ChatSessionManagementService>>();

    private ChatSessionManagementService CreateService() =>
        new ChatSessionManagementService(
            _loggerMock.Object,
            _chatSessionRepositoryMock.Object,
            _teamRepositoryMock.Object,
            _agentManagementServiceMock.Object);

    [Fact]
    public async Task AssignChatSessionAsync_AgentAvailable_ReturnsAssignedTrue()
    {
        // Arrange
        var agentName = "Test Agent";
        var agentSeniority = AgentSeniority.Junior; // Use an appropriate value for your enum
        var shiftId = Guid.NewGuid(); // Simulate a valid ShiftId
        var teamId = Guid.NewGuid(); // Simulate a valid TeamId
        var agentId = Guid.NewGuid(); // ID for the mock agent
        var chatSessionId = Guid.NewGuid();
        
        var chatSession = new ChatSession
        { 
            Id = chatSessionId,
            Status = ChatSessionStatus.Waiting
        };

        _chatSessionRepositoryMock.Setup(repo => repo.GetByIdAsync(chatSessionId))
                                  .ReturnsAsync(chatSession);

        _agentManagementServiceMock.Setup(service => service.CalculateCapacityAndMaxQueueLengthAsync())
                                   .ReturnsAsync((10, 15)); // Simulate available capacity

        var agent = new Agent(agentName, agentSeniority, shiftId, teamId)
        {
            Id = agentId
        };

        _agentManagementServiceMock.Setup(service => service.GetNextAvailableAgent(It.IsAny<DateTime>()))
                                   .ReturnsAsync(agent);

        var service = CreateService();

        // Act
        var (isAssigned, assignedAgentId) = await service.AssignChatSessionAsync(chatSessionId);

        // Assert
        Assert.True(isAssigned);
        Assert.Equal(agentId, assignedAgentId);

        _chatSessionRepositoryMock.Verify(repo => repo.GetByIdAsync(chatSessionId), Times.Once);
    }

    [Fact]
    public async Task AssignChatSessionAsync_QueueFull_ReturnsAssignedFalse()
    {
        // Arrange
        var chatSessionId = Guid.NewGuid();
        var chatSession = new ChatSession { Id = chatSessionId, Status = ChatSessionStatus.Waiting };

        _chatSessionRepositoryMock.Setup(repo => repo.GetByIdAsync(chatSessionId))
                                  .ReturnsAsync(chatSession);

        _chatSessionRepositoryMock.Setup(repo => repo.GetActiveAndWaitingChatSessionsCountAsync())
                                  .ReturnsAsync(16); // Simulate queue being full

        _agentManagementServiceMock.Setup(service => service.CalculateCapacityAndMaxQueueLengthAsync())
                                   .ReturnsAsync((10, 15)); // Simulate max queue length

        var service = CreateService();

        // Act
        var (isAssigned, assignedAgentId) = await service.AssignChatSessionAsync(chatSessionId);

        // Assert
        Assert.False(isAssigned);
        Assert.Null(assignedAgentId); // No agent should be assigned as the queue is full

        _chatSessionRepositoryMock.Verify(repo => repo.GetByIdAsync(chatSessionId), Times.Once);
        // Ensure no attempt to update the chat session was made since it wasn't assigned
        _chatSessionRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<ChatSession>()), Times.Never);
    }
}
