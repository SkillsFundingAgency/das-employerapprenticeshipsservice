using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.Support.Shared.SiteConnection;

namespace SFA.DAS.EAS.Support.Web.Configuration;

public interface IWebConfiguration
{
    AccountApiConfiguration AccountApi { get; set; }
    LevySubmissionsSettings LevySubmission { get; set; }
    HashingServiceConfig HashingService { get; set; }
    EmployerAccountsConfiguration EmployerAccountsConfiguration { get; set; }
}