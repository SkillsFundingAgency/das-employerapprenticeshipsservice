using System;
using System.Net.Http;
using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ApprenticeshipLevyRegistry : Registry
    {
        public ApprenticeshipLevyRegistry()
        {
            For<IApprenticeshipLevyApiClient>().Use<ApprenticeshipLevyApiClient>().Ctor<HttpClient>().Is(
                c => new HttpClient { BaseAddress = new Uri(c.GetInstance<EmployerFinanceConfiguration>().Hmrc.BaseUrl) }
            );
        }
    }
}
