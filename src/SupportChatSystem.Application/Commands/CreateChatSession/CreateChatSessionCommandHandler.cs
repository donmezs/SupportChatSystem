using AutoMapper;
using MediatR;
using RabbitMQ.Client;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Application.Interfaces;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Repositories.Abstactions;
using System.Text;
using System.Text.Json;

namespace SupportChatSystem.Application.Commands.CreateChatSession;
public class CreateChatSessionCommandHandler : IRequestHandler<CreateChatSessionCommand, ChatSessionDto>
{
    private readonly IMapper _mapper;
    private readonly IModel _rabbitMQChannel;
    private readonly IChatSessionManagementService _chatSessionManagementService;
    public CreateChatSessionCommandHandler(IMapper mapper, IModel rabbitMQChannel, IChatSessionManagementService chatSessionManagementService)
    {
        _mapper = mapper;
        _rabbitMQChannel = rabbitMQChannel;
        _chatSessionManagementService = chatSessionManagementService;
    }

    public async Task<ChatSessionDto> Handle(CreateChatSessionCommand request, CancellationToken cancellationToken)
    {
        var chatSession = await _chatSessionManagementService.CreateChatSessionAsync();

        if (chatSession == null)
        {
            return null;
        }

        PublishChatSessionCreated(chatSession);

        return _mapper.Map<ChatSessionDto>(chatSession);
    }

    private void PublishChatSessionCreated(ChatSession chatSession)
    {
        _rabbitMQChannel.QueueDeclare(queue: "chat_sessions_queue",
                                      durable: false,
                                      exclusive: false,
                                      autoDelete: true,
                                      arguments: null);

        var message = JsonSerializer.Serialize(new { ChatSessionId = chatSession.Id });
        var body = Encoding.UTF8.GetBytes(message);

        _rabbitMQChannel.BasicPublish(exchange: "",
                                      routingKey: "chat_sessions_queue",
                                      basicProperties: null,
                                      body: body);
    }
}

