using MediatR;
using SupportChatSystem.Application.DTOs;

namespace SupportChatSystem.Application.Queries.GetChatSession;
public class GetChatSessionQuery : IRequest<ChatSessionDto>
{
    public Guid ChatSessionId { get; set; }

    public GetChatSessionQuery(Guid chatSessionId)
    {
        ChatSessionId = chatSessionId;
    }
}
