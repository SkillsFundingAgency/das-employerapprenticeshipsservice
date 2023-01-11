using System.Net.Http;
using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class ApprenticeshipLevyRegistry : Registry
{
    public ApprenticeshipLevyRegistry()
    {
        For<IApprenticeshipLevyApiClient>().Use<ApprenticeshipLevyApiClient>().Ctor<HttpClient>().Is(
            c => new HttpClient { BaseAddress = new Uri(c.GetInstance<EmployerAccountsConfiguration>().Hmrc.BaseUrl) }
        );
    }

}