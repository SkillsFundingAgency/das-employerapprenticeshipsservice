using System.Runtime.Serialization;

namespace SFA.DAS.EmployerAccounts.Audit;

[Serializable]
public class InvalidContextException : Exception
{
    public InvalidContextException(string message) : base(message) { }

    protected InvalidContextException(SerializationInfo  info, StreamingContext context) : base(info, context) { }
}
