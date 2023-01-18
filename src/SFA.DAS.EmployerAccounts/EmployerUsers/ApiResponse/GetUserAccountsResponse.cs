using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.EmployerUsers.ApiResponse;

public class GetUserAccountsResponse
{
    [JsonProperty]
    public string EmployerUserId { get; set; }
    [JsonProperty]
    public string FirstName { get; set; }
    [JsonProperty]
    public string LastName { get; set; }
    [JsonProperty]
    public string Email { get; set; }
    [JsonProperty("UserAccounts")]
    public List<EmployerIdentifier> UserAccounts { get; set; }
}