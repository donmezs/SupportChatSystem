namespace SupportChatSystem.Domain.Exceptions;
public class BaseDomainException : Exception
{
    public BaseDomainException()
    {
    }

    public BaseDomainException(string message)
        : base(message)
    {
    }

    public BaseDomainException(string message, Exception inner)
        : base(message, inner)
    {
    }
}