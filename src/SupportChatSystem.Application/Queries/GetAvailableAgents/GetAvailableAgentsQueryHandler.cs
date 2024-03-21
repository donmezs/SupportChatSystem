using AutoMapper;
using MediatR;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Domain.Repositories.Abstactions;

namespace SupportChatSystem.Application.Queries.GetAvailableAgents;
public class GetAvailableAgentsQueryHandler : IRequestHandler<GetAvailableAgentsQuery, IEnumerable<AgentDto>>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IMapper _mapper;

    public GetAvailableAgentsQueryHandler(IAgentRepository agentRepository, IMapper mapper)
    {
        _agentRepository = agentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AgentDto>> Handle(GetAvailableAgentsQuery request, CancellationToken cancellationToken)
    {
        var availableAgents = await _agentRepository.GetAvailableAgentsAsync();
        return _mapper.Map<IEnumerable<AgentDto>>(availableAgents);
    }
}
