namespace SupportChatSystem.Domain.Exceptions;
public class TeamNameInvalidException : BaseDomainException
{
    public TeamNameInvalidException(string message) : base(message)
    {
    }
}
