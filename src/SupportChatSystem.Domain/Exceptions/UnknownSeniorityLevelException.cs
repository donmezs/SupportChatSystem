namespace SupportChatSystem.Domain.Exceptions;
public class UnknownSeniorityLevelException : BaseDomainException
{
    public UnknownSeniorityLevelException(string message) : base(message)
    {
    }
}
