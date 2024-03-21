namespace SupportChatSystem.Application.BackgroundServices.Consumers.Interfaces;
public interface IAgentQueueConsumer
{
    Task ProcessMessageAsync(string message, Guid agentId, Guid chatSessionId);
}
