﻿using System;
using System.Net.Http;
using HMRC.ESFA.Levy.Api.Client;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class ApprenticeshipLevyRegistry : Registry
    {
        public ApprenticeshipLevyRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<EmployerAccountsConfiguration>(Constants.ServiceName);
            var httpClient = new HttpClient {BaseAddress = new Uri(config.Hmrc.BaseUrl)};
            For<IApprenticeshipLevyApiClient>().Use<ApprenticeshipLevyApiClient>().Ctor<HttpClient>().Is(httpClient);
        }

    }
}
