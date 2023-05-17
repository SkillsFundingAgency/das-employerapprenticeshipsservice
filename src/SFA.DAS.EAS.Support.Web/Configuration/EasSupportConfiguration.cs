using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.Infrastructure.Settings;

namespace SFA.DAS.EAS.Support.Web.Configuration;

public class EasSupportConfiguration : IEasSupportConfiguration
{
    [JsonRequired]
    public AccountApiConfiguration AccountApi { get; set; }

    [JsonRequired] 
    public SiteValidatorSettings SiteValidator { get; set; }

    [JsonRequired]
    public LevySubmissionsSettings LevySubmission { get; set; }

    [JsonRequired]
    public HashingServiceConfig HashingService { get; set; }

    [JsonRequired]
    public EmployerAccountsConfiguration EmployerAccountsConfiguration { get; set; }
}