using MediatR;

namespace SupportChatSystem.Application.Commands.MarkChatSessionInactive;
public class MarkChatSessionInactiveCommand : IRequest<bool>
{
    public Guid ChatSessionId { get; set; }

    public MarkChatSessionInactiveCommand(Guid chatSessionId)
    {
        ChatSessionId = chatSessionId;
    }
}
