using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Exceptions;

[Serializable]
public class InvalidRequestException : Exception
{
    public Dictionary<string, string> ErrorMessages { get; private set; }

    public InvalidRequestException(Dictionary<string, string> errorMessages) : base(BuildErrorMessage(errorMessages))
    {
        ErrorMessages = errorMessages;
    }

    protected InvalidRequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    private static string BuildErrorMessage(Dictionary<string, string> errorMessages)
    {
        if (!errorMessages.Any())
        {
            errorMessages.Add("Request", "Request is invalid");
        }
        return JsonConvert.SerializeObject(errorMessages, Formatting.Indented);
    }
}