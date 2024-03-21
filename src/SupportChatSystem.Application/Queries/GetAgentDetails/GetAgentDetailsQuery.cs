using MediatR;
using SupportChatSystem.Application.DTOs;

namespace SupportChatSystem.Application.Queries.GetAgentDetails;
public class GetAgentDetailsQuery : IRequest<AgentDto>
{
    public Guid AgentId { get; set; }

    public GetAgentDetailsQuery(Guid agentId)
    {
        AgentId = agentId;
    }
}
