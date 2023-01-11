namespace SFA.DAS.EmployerAccounts.Exceptions;

[Serializable]
public class ServiceTimeoutException : Exception
{
    public ServiceTimeoutException() { }
    public ServiceTimeoutException(string message) : base(message) { }
    public ServiceTimeoutException(string message, Exception inner) : base(message, inner) { }
    protected ServiceTimeoutException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}