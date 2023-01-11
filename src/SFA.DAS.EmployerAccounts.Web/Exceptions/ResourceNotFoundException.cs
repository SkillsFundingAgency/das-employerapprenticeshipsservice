using System.Runtime.Serialization;

namespace SFA.DAS.EmployerAccounts.Web.Exceptions;

[Serializable]
public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException() { }
    public ResourceNotFoundException(string message) : base(message) { }
    public ResourceNotFoundException(string message, Exception inner) : base(message, inner) { }
    protected ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}