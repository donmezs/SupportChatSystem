using FluentAssertions;
using Moq;
using SupportChatSystem.Application.Commands.MarkChatSessionInactive;
using SupportChatSystem.Application.Interfaces;

namespace SupportChatSystem.Tests.CommandHandlers;
public class MarkChatSessionInactiveCommandHandlerTests
{
    [Fact]
    public async Task Handle_GivenValidChatSessionId_ShouldMarkSessionInactive()
    {
        // Arrange
        var chatSessionId = Guid.NewGuid();
        var mockChatSessionManagementService = new Mock<IChatSessionManagementService>();
        mockChatSessionManagementService.Setup(x => x.MarkInactiveSessionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true); // Simulate successful marking of the session as inactive

        var handler = new MarkChatSessionInactiveCommandHandler(mockChatSessionManagementService.Object);

        // Act
        var result = await handler.Handle(new MarkChatSessionInactiveCommand(chatSessionId), CancellationToken.None);

        // Assert
        result.Should().BeTrue(); // Assert that the session was marked inactive successfully
        mockChatSessionManagementService.Verify(x => x.MarkInactiveSessionAsync(chatSessionId), Times.Once); // Verify that the method was called once with the correct chatSessionId
    }

    [Fact]
    public async Task Handle_GivenInvalidChatSessionId_ShouldReturnFalse()
    {
        // Arrange
        var chatSessionId = Guid.NewGuid();
        var mockChatSessionManagementService = new Mock<IChatSessionManagementService>();
        mockChatSessionManagementService.Setup(x => x.MarkInactiveSessionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false); // Simulate failure in marking the session as inactive

        var handler = new MarkChatSessionInactiveCommandHandler(mockChatSessionManagementService.Object);

        // Act
        var result = await handler.Handle(new MarkChatSessionInactiveCommand(chatSessionId), CancellationToken.None);

        // Assert
        result.Should().BeFalse(); // Assert that the session was not marked inactive
        mockChatSessionManagementService.Verify(x => x.MarkInactiveSessionAsync(chatSessionId), Times.Once); // Verify that the method was called once with the correct chatSessionId
    }
}
