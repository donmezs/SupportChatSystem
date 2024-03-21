namespace SupportChatSystem.Domain.Exceptions;
public class AgentNameInvalidException : BaseDomainException
{
    public AgentNameInvalidException(string message) : base(message)
    {
    }
}
