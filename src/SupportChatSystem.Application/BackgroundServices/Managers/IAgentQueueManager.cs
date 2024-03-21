namespace SupportChatSystem.Application.BackgroundServices.Managers;
public interface IAgentQueueManager
{
    void EnsureListening(Guid agentId);

    void PublishChatSessionToAgentQueue(Guid chatSessionId, Guid agentId);
}
