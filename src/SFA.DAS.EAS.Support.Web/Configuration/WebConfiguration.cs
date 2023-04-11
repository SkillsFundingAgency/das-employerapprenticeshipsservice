using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.Support.Shared.SiteConnection;

namespace SFA.DAS.EAS.Support.Web.Configuration
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public AccountApiConfiguration AccountApi { get; set; }

        [JsonRequired] public SiteValidatorSettings SiteValidator { get; set; }

        [JsonRequired] public LevySubmissionsSettings LevySubmission { get; set; }

        [JsonRequired] public HashingServiceConfig HashingService { get; set; }

        [JsonRequired] public EmployerAccountsConfiguration EmployerAccountsConfiguration { get; set; }

        [JsonRequired] public bool UseDfESignIn { get; set; } = false;
    }
}