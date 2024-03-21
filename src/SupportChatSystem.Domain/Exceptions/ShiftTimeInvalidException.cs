namespace SupportChatSystem.Domain.Exceptions;
public class ShiftTimeInvalidException : BaseDomainException
{
    public ShiftTimeInvalidException(string message) : base(message)
    {
    }
}
