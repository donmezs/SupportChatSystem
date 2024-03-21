using MediatR;
using SupportChatSystem.Application.DTOs;

namespace SupportChatSystem.Application.Queries.GetAvailableAgents;
public class GetAvailableAgentsQuery : IRequest<IEnumerable<AgentDto>>
{
}
