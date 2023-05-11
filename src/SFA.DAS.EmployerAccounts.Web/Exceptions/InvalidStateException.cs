using System.Runtime.Serialization;

namespace SFA.DAS.EmployerAccounts.Web.Exceptions;

[Serializable]
public class InvalidStateException : Exception
{
    public InvalidStateException(string message) : base(message) { }

    protected InvalidStateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}