using MediatR;
using SupportChatSystem.Application.DTOs;

namespace SupportChatSystem.Application.Commands.CreateChatSession;
public class CreateChatSessionCommand : IRequest<ChatSessionDto>
{
}
