using Microsoft.Extensions.Logging;
using SupportChatSystem.Application.Exceptions;
using SupportChatSystem.Application.Interfaces;
using SupportChatSystem.Domain.Entities;
using SupportChatSystem.Domain.Enums;
using SupportChatSystem.Domain.Repositories.Abstactions;

namespace SupportChatSystem.Application.Services;
public class ChatSessionManagementService : IChatSessionManagementService
{
    private readonly ILogger<ChatSessionManagementService> _logger;
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IAgentManagementService _agentManagementService;

    public ChatSessionManagementService(
        ILogger<ChatSessionManagementService> logger,
        IChatSessionRepository chatSessionRepository,
        ITeamRepository teamRepository,
        IAgentManagementService agentManagementService)
    {
        _logger = logger;
        _chatSessionRepository = chatSessionRepository;
        _teamRepository = teamRepository;
        _agentManagementService = agentManagementService;
    }

    public async Task<ChatSession> CreateChatSessionAsync()
    {
        // Calculate total capacity and maximum queue length
        (int totalCapacity, int maximumQueueLength) = await _agentManagementService.CalculateCapacityAndMaxQueueLengthAsync();

        // Get current active (waiting or active status) queue length
        var currentQueueLength = await _chatSessionRepository.GetActiveAndWaitingChatSessionsCountAsync();

        // Check if queue is full
        if (currentQueueLength >= maximumQueueLength)
        {
            if (_agentManagementService.IsWithinOfficeHours())
            {
                // Attempt to assign to overflow team if within office hours
                var overflowTeamAvailable = await IsOverflowTeamAvailable();
                if (!overflowTeamAvailable)
                {
                    _logger.LogInformation("Chat session refused: Queue is full and overflow is not available.");
                    throw new OverflowUnavailableException("Overflow team is unavailable or full.");// Refuse the chat session creation as overflow is also full
                }
            }
            else
            {
                _logger.LogInformation("Chat session refused: Queue is full and it's outside office hours.");
                throw new ChatSessionRefusedException("Chat session creation refused due to queue being full."); // Refuse the chat session creation as it's outside office hours and queue is full
            }
        }

        var newSession = new ChatSession
        {
            StartTime = DateTime.UtcNow,
            Status = ChatSessionStatus.Waiting
        };

        await _chatSessionRepository.AddAsync(newSession);
        return newSession;
    }

    public async Task<bool> MarkInactiveSessionAsync(Guid chatSessionId)
    {
        var session = await _chatSessionRepository.GetByIdAsync(chatSessionId);
        var markingInactiveCondition = session.LastPolledTime.HasValue &&
                                        session.Status != ChatSessionStatus.Active;
        if (markingInactiveCondition)
        {
            session.Status = ChatSessionStatus.Inactive;
            await _chatSessionRepository.UpdateAsync(session);
            return true;
        }

        return false;
    }

    public async Task<bool> SetChatSessionStatus(Guid chatSessionId, ChatSessionStatus status)
    {
        var session = await _chatSessionRepository.GetByIdAsync(chatSessionId);

        if (session is null)
        {
            return false;
        }
        else
        {
            session.Status = status;
            await _chatSessionRepository.UpdateAsync(session);
            return true;
        }
    }

    public async Task<(bool IsAssigned, Guid? AssignedAgentId)> AssignChatSessionAsync(Guid chatSessionId)
    {
        var chatSession = await _chatSessionRepository.GetByIdAsync(chatSessionId);
        if (chatSession == null || chatSession.Status != ChatSessionStatus.Waiting)
        {
            return (false, null);
        }

        // Calculate total capacity and maximum queue length using the new method
        (int totalCapacity, int maximumQueueLength) = await _agentManagementService.CalculateCapacityAndMaxQueueLengthAsync();

        // Get current queue length
        var currentQueueLength = await _chatSessionRepository.GetActiveAndWaitingChatSessionsCountAsync();

        if (currentQueueLength < maximumQueueLength)
        {
            var (isAssigned, agentId) = await TryAssignToRegularAgents(chatSession);
            if (isAssigned)
            {
                return (true, agentId);
            }
        }

        // Attempt to assign to overflow agents if within office hours
        return await TryAssignToOverflowAgents(chatSession);
    }

    private async Task<(bool IsAssigned, Guid? AssignedAgentId)> TryAssignToRegularAgents(ChatSession chatSession)
    {
        var nowUtc = DateTime.UtcNow;
        var availableAgent = await _agentManagementService.GetNextAvailableAgent(nowUtc);

        if (availableAgent != null)
        {
            return (true, availableAgent.Id); ; // Successfully assigned to an available regular agent
        }

        return (false, null); ; // No regular agent was available for assignment
    }

    private async Task<(bool IsAssigned, Guid? AssignedAgentId)> TryAssignToOverflowAgents(ChatSession chatSession)
    {
        if (!_agentManagementService.IsWithinOfficeHours())
        {
            _logger.LogInformation("Attempted to assign to overflow outside of office hours.");
            return (false, null);
        }

        var overflowTeam = await _teamRepository.GetOverflowTeamAsync();
        if (overflowTeam == null || !overflowTeam.Agents.Any()) return (false, null);

        var nowUtc = DateTime.UtcNow;
        var teamCapacity = overflowTeam.Agents.Sum(agent => _agentManagementService.CalculateAgentCapacity(agent));
        var currentLoad = overflowTeam.Agents.Sum(agent => agent.ChatSessions.Count(cs => cs.Status == ChatSessionStatus.Active));

        // Check if the team's current load exceeds its capacity
        if (currentLoad >= teamCapacity)
        {
            _logger.LogInformation("Overflow team capacity exceeded. Team ID: {0}", overflowTeam.Id);
            return (false, null);
        }

        foreach (var agent in overflowTeam.Agents.OrderBy(a => Guid.NewGuid()))
        {
            if (_agentManagementService.IsAgentAvailable(agent))
            {
                _logger.LogInformation("Assigned chat session {0} to overflow agent {1}", chatSession.Id, agent.Id);
                return (true, agent.Id);
            }
        }

        return (false, null); // No overflow agent was available for assignment
    }

    private async Task<bool> IsOverflowTeamAvailable()
    {
        var overflowTeam = await _teamRepository.GetOverflowTeamAsync();
        if (overflowTeam == null) return false;

        // Calculate overflow team's capacity and current load
        var teamCapacity = overflowTeam.Agents.Sum(agent => _agentManagementService.CalculateAgentCapacity(agent));
        var currentLoad = overflowTeam.Agents.Sum(agent => agent.ChatSessions.Count(cs => cs.Status == ChatSessionStatus.Active || cs.Status == ChatSessionStatus.Waiting));

        // Check if overflow team has capacity
        return currentLoad < teamCapacity;
    }
}

