using System.Runtime.Serialization;

namespace SFA.DAS.EmployerAccounts.Exceptions.Hmrc;

[Serializable]
public class InternalServerErrorException : HttpException
{
    public InternalServerErrorException() : base(500, "Internal server error") { }

    protected InternalServerErrorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}