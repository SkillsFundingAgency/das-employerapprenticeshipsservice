using System.Runtime.Serialization;

namespace SFA.DAS.EmployerAccounts.Exceptions;

[Serializable]
public class UnsubscribeNotificationException : Exception
{
    public UnsubscribeNotificationException(string message): base(message) { }

    protected UnsubscribeNotificationException(SerializationInfo info, StreamingContext context) : base(info, context){}
}