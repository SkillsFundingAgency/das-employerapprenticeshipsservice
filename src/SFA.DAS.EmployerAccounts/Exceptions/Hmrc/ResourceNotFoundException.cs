using System.Runtime.Serialization;

namespace SFA.DAS.EmployerAccounts.Exceptions.Hmrc;

[Serializable]
public class ResourceNotFoundException : HttpException
{
    public ResourceNotFoundException(string resourceUri) : base(404, "Could not find requested resource - " + resourceUri) { }

    protected  ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}