using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportChatSystem.Application.DTOs;
using SupportChatSystem.Application.Queries.GetAgentDetails;
using SupportChatSystem.Application.Queries.GetAvailableAgents;

namespace SupportChatSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AgentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AgentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<AgentDto>>> GetAvailableAgents()
    {
        var query = new GetAvailableAgentsQuery();
        var agents = await _mediator.Send(query);
        return Ok(agents);
    }

    [HttpGet("{agentId}")]
    public async Task<ActionResult<AgentDto>> GetAgentDetails(Guid agentId)
    {
        var query = new GetAgentDetailsQuery(agentId);
        var agentDetails = await _mediator.Send(query);

        if (agentDetails == null)
        {
            return NotFound($"Agent with ID {agentId} not found.");
        }

        return Ok(agentDetails);
    }
}
