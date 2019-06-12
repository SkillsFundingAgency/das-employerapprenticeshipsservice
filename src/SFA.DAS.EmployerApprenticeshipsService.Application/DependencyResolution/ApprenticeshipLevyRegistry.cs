using System;
using System.Net.Http;
using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.EAS.Domain.Configuration;
using StructureMap;


namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ApprenticeshipLevyRegistry : Registry
    {
        public ApprenticeshipLevyRegistry()
        {
            For<IApprenticeshipLevyApiClient>().Use<ApprenticeshipLevyApiClient>().Ctor<HttpClient>().Is(c => new HttpClient{ BaseAddress = new Uri(c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().Hmrc.BaseUrl) });
        }

    }
}
