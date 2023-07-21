using System.Runtime.Serialization;

namespace SFA.DAS.EmployerAccounts.Exceptions.Hmrc;

[Serializable]
public class ServiceUnavailableException : HttpException
{
    public ServiceUnavailableException() : base(503, "Service is unavailable") { }

    protected ServiceUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}