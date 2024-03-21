using AutoMapper;
using MediatR;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Domain.Repositories.Abstactions;

namespace SupportChatSystem.Application.Queries.GetAgentDetails;
public class GetAgentDetailsQueryHandler : IRequestHandler<GetAgentDetailsQuery, AgentDto>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IMapper _mapper;

    public GetAgentDetailsQueryHandler(IAgentRepository agentRepository, IMapper mapper)
    {
        _agentRepository = agentRepository;
        _mapper = mapper;
    }

    public async Task<AgentDto> Handle(GetAgentDetailsQuery request, CancellationToken cancellationToken)
    {
        var agent = await _agentRepository.GetByIdAsync(request.AgentId);
        if (agent == null)
        {
            // Handle the case where the agent is not found.
            // This could involve returning null, or perhaps throwing a not found exception,
            // depending on how you prefer to handle such cases.
            return null; // or throw new EntityNotFoundException("Agent not found.");
        }

        return _mapper.Map<AgentDto>(agent);
    }
}
