namespace SupportChatSystem.Application.Exceptions;
public class ChatSessionRefusedException : BaseApplicationException
{
    public ChatSessionRefusedException(string message) : base(message)
    {
    }
}
