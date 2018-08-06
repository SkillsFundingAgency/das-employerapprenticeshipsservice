using System;
using System.Net.Http;
using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.EmployerFinance.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ApprenticeshipLevyRegistry : Registry
    {
        public ApprenticeshipLevyRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<EmployerFinanceConfiguration>("SFA.DAS.EmployerFinance");
            var httpClient = new HttpClient { BaseAddress = new Uri(config.Hmrc.BaseUrl) };

            For<IApprenticeshipLevyApiClient>().Use<ApprenticeshipLevyApiClient>().Ctor<HttpClient>().Is(httpClient);
        }
    }
}
