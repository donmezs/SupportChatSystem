using AutoMapper;
using MediatR;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Domain.Repositories.Abstactions;

namespace SupportChatSystem.Application.Queries.ListActiveChatSessions;
public class ListActiveChatSessionsQueryHandler : IRequestHandler<ListActiveChatSessionsQuery, IEnumerable<ChatSessionDto>>
{
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IMapper _mapper;

    public ListActiveChatSessionsQueryHandler(IChatSessionRepository chatSessionRepository, IMapper mapper)
    {
        _chatSessionRepository = chatSessionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ChatSessionDto>> Handle(ListActiveChatSessionsQuery request, CancellationToken cancellationToken)
    {
        var activeChatSessions = await _chatSessionRepository.GetActiveChatSessionsAsync();
        return _mapper.Map<IEnumerable<ChatSessionDto>>(activeChatSessions);
    }
}
