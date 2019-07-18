using System;
using System.Net.Http;
using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.Configuration;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using StructureMap;


namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ApprenticeshipLevyRegistry : Registry
    {
        public ApprenticeshipLevyRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName);
            var httpClient = new HttpClient {BaseAddress = new Uri(config.Hmrc.BaseUrl)};
            For<IApprenticeshipLevyApiClient>().Use<ApprenticeshipLevyApiClient>().Ctor<HttpClient>().Is(httpClient);
        }

    }
}
