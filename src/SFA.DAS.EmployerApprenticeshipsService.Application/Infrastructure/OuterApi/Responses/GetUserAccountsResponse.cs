using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Serialization.Converters;

namespace SFA.DAS.EAS.Application.Infrastructure.OuterApi.Responses;

public class GetUserAccountsResponse
{
    [JsonProperty(PropertyName = "employerUserId")]
    public string EmployerUserId { get; set; }
    [JsonProperty(PropertyName = "firstName")]
    public string FirstName { get; set; }
    [JsonProperty(PropertyName = "lastName")]
    public string LastName { get; set; }
    [JsonProperty(PropertyName = "userAccounts")]
    public List<EmployerIdentifier> UserAccounts { get; set; }
    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }
    [JsonProperty(PropertyName = "isSuspended")]
    public bool IsSuspended { get; set; }
}

public class EmployerIdentifier
{
    [JsonProperty("encodedAccountId")]
    public string AccountId { get; set; }
    [JsonProperty("dasAccountName")]
    public string EmployerName { get; set; }
    [JsonProperty("role")]
    public string Role { get; set; }
    [JsonProperty("apprenticeshipEmployerType")]
    [Newtonsoft.Json.JsonConverter(typeof(ApprenticeshipEmployerTypeConverter))]
    public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
}
