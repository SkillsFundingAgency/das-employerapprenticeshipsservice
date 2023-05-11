using System.Runtime.Serialization;

namespace SFA.DAS.EmployerAccounts.Exceptions;

[Serializable]
public class InvalidOrganisationTypeConversionException : Exception
{
    public InvalidOrganisationTypeConversionException(string message) : base(message) { }

    protected InvalidOrganisationTypeConversionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}