using System.Runtime.Serialization;

namespace SFA.DAS.EmployerAccounts.Exceptions;

[Serializable]
public class InvalidConfigurationValueException: Exception
{
    public InvalidConfigurationValueException(string configurationItem) : base($"Configuration value for '{configurationItem}' is not valid.") { }

    protected InvalidConfigurationValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}