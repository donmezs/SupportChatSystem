using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupportChatSystem.Application.Commands.MarkChatSessionInactive;
using SupportChatSystem.Domain.Repositories.Abstactions;

namespace SupportChatSystem.Application.BackgroundServices.Services;
public class PollingService : BackgroundService
{
    private readonly ILogger<PollingService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Chat Session Monitor starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await MonitorChatSessions(stoppingToken);
            await Task.Delay(1000, stoppingToken); // Wait for 1 second before the next check
        }

        _logger.LogInformation("Chat Session Monitor stopping.");
    }

    private async Task MonitorChatSessions(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var chatSessionRepository = scope.ServiceProvider.GetRequiredService<IChatSessionRepository>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var activeSessions = await chatSessionRepository.GetActiveChatSessionsAsync();
            var currentTime = DateTime.UtcNow;

            foreach (var session in activeSessions.Where(s => s.LastPolledTime.HasValue))
            {
                var timeSinceLastPoll = currentTime - session.LastPolledTime.Value;

                // Mark as inactive if the session hasn't been polled for more than 3 seconds
                if (timeSinceLastPoll.TotalSeconds > 3)
                {
                    await mediator.Send(new MarkChatSessionInactiveCommand(session.Id), stoppingToken);
                    _logger.LogInformation($"Chat session {session.Id} marked as inactive due to inactivity.");
                }
            }
        }
    }
}