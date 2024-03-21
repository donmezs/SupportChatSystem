namespace SupportChatSystem.Application.Exceptions;
public class AgentUnavailableException : BaseApplicationException
{
    public AgentUnavailableException(string message) : base(message)
    {
    }
}
