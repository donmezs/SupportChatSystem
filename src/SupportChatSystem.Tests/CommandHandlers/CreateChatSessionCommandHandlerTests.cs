using AutoMapper;
using FluentAssertions;
using Moq;
using RabbitMQ.Client;
using SupportChatSystem.Application.Commands.CreateChatSession;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Application.Interfaces;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;

namespace SupportChatSystem.Tests.CommandHandlers;

public class CreateChatSessionCommandHandlerTests
{
    [Fact]
    public async Task Handle_Success_ShouldCreateChatSessionAndReturnDto()
    {
        // Arrange
        var chatSessionManagementServiceMock = new Mock<IChatSessionManagementService>();
        var mapperMock = new Mock<IMapper>();
        var modelMock = new Mock<IModel>();

        var chatSession = new ChatSession
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            Status = ChatSessionStatus.Waiting
        };

        chatSessionManagementServiceMock
            .Setup(x => x.CreateChatSessionAsync())
            .ReturnsAsync(chatSession);

        var chatSessionDto = new ChatSessionDto
        {
            Id = chatSession.Id,
            StartTime = chatSession.StartTime,
            Status = chatSession.Status
        };

        mapperMock
            .Setup(m => m.Map<ChatSessionDto>(It.IsAny<ChatSession>()))
            .Returns(chatSessionDto);

        var handler = new CreateChatSessionCommandHandler(mapperMock.Object, modelMock.Object, chatSessionManagementServiceMock.Object);

        // Act
        var result = await handler.Handle(new CreateChatSessionCommand(), default);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(chatSession.Id);
        result.Status.Should().Be(chatSession.Status);

        chatSessionManagementServiceMock.Verify(x => x.CreateChatSessionAsync(), Times.Once);
        mapperMock.Verify(m => m.Map<ChatSessionDto>(It.IsAny<ChatSession>()), Times.Once);
    }

    [Fact]
    public async Task TestChatSessionCreationFailure_ShouldReturnNull()
    {
        // Arrange
        var mockChatSessionManagementService = new Mock<IChatSessionManagementService>();
        var mockMapper = new Mock<IMapper>();
        var mockModel = new Mock<IModel>();

        // Setup the mock to return null to simulate failure in chat session creation
        mockChatSessionManagementService
            .Setup(service => service.CreateChatSessionAsync())
            .ReturnsAsync((ChatSession)null);

        // Create an instance of the command handler with the mocked dependencies
        var commandHandler = new CreateChatSessionCommandHandler(
            mockMapper.Object,
            mockModel.Object,
            mockChatSessionManagementService.Object);

        // Act
        var result = await commandHandler.Handle(new CreateChatSessionCommand(), default);

        // Assert
        result.Should().BeNull("Because the chat session creation failed.");

        // Verify that CreateChatSessionAsync was called once
        mockChatSessionManagementService.Verify(
            service => service.CreateChatSessionAsync(), Times.Once);
    }
}
