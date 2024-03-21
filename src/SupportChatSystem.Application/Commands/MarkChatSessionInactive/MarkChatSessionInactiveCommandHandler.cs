using MediatR;
using SupportChatSystem.Application.Interfaces;

namespace SupportChatSystem.Application.Commands.MarkChatSessionInactive;
public class MarkChatSessionInactiveCommandHandler : IRequestHandler<MarkChatSessionInactiveCommand, bool>
{
    private readonly IChatSessionManagementService _chatSessionManagementService;

    public MarkChatSessionInactiveCommandHandler(IChatSessionManagementService chatSessionManagementService)
    {
        _chatSessionManagementService = chatSessionManagementService;
    }

    public async Task<bool> Handle(MarkChatSessionInactiveCommand request, CancellationToken cancellationToken)
    {
        return await _chatSessionManagementService.MarkInactiveSessionAsync(request.ChatSessionId);
    }
}
