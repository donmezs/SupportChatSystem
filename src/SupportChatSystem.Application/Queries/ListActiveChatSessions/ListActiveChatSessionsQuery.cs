using MediatR;
using SupportChatSystem.Application.DTOs;

namespace SupportChatSystem.Application.Queries.ListActiveChatSessions;
public class ListActiveChatSessionsQuery : IRequest<IEnumerable<ChatSessionDto>>
{
}
