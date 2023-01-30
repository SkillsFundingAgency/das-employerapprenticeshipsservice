using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Exceptions;

[Serializable]
public class InvalidRequestException : Exception
{        
    public InvalidRequestException() { }

    public InvalidRequestException(Dictionary<string, string> errorMessages): base(BuildErrorMessage(errorMessages))
    {
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