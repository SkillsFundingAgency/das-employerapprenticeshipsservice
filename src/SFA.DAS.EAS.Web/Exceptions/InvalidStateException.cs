namespace SFA.DAS.EAS.Web.Exceptions;

[Serializable]
public class InvalidStateException : Exception
{
    public InvalidStateException(string message) : base(message) { }

    protected InvalidStateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}