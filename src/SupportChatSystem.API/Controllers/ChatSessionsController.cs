using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportChatSystem.Application.Commands.CreateChatSession;
using SupportChatSystem.Application.Commands.MarkChatSessionInactive;
using SupportChatSystem.Application.Queries.GetChatSession;
using SupportChatSystem.Application.Queries.ListActiveChatSessions;

namespace SupportChatSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatSessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatSessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create()
    {
        var command = new CreateChatSessionCommand();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{chatSessionId}/markInactive")]
    public async Task<IActionResult> MarkInactive([FromRoute] Guid chatSessionId)
    {
        var command = new MarkChatSessionInactiveCommand(chatSessionId);
        var success = await _mediator.Send(command);

        return success ? Ok() : NotFound("Chat session not found or already inactive.");
    }

    [HttpGet("{chatSessionId}")]
    public async Task<IActionResult> GetChatSession([FromRoute] Guid chatSessionId)
    {
        var query = new GetChatSessionQuery(chatSessionId);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound("Chat session not found.");
        }
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> ListActiveChatSessions()
    {
        var query = new ListActiveChatSessionsQuery();
        var results = await _mediator.Send(query);
        return Ok(results);
    }
}