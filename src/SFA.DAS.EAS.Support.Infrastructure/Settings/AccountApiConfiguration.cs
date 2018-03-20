using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.EAS.Support.Infrastructure.Settings
{
    [ExcludeFromCodeCoverage]
    public class AccountApiConfiguration : IAccountApiConfiguration
    {
        [JsonRequired] public string ApiBaseUrl { get; set; }

        [JsonRequired] public string ClientId { get; set; }

        [JsonRequired] public string ClientSecret { get; set; }

        [JsonRequired] public string IdentifierUri { get; set; }

        [JsonRequired] public string Tenant { get; set; }
    }
}