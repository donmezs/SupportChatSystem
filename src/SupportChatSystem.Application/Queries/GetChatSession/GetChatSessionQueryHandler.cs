using AutoMapper;
using MediatR;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Domain.Repositories.Abstactions;

namespace SupportChatSystem.Application.Queries.GetChatSession;
public class GetChatSessionQueryHandler : IRequestHandler<GetChatSessionQuery, ChatSessionDto>
{
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IMapper _mapper;

    public GetChatSessionQueryHandler(IChatSessionRepository chatSessionRepository, IMapper mapper)
    {
        _chatSessionRepository = chatSessionRepository;
        _mapper = mapper;
    }

    public async Task<ChatSessionDto> Handle(GetChatSessionQuery request, CancellationToken cancellationToken)
    {
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId);
        if (chatSession == null)
        {
            // Optionally, handle the not found scenario, e.g., by returning null or throwing an exception
            return null; // Or throw new NotFoundException("ChatSession not found.");
        }

        return _mapper.Map<ChatSessionDto>(chatSession);
    }
}
