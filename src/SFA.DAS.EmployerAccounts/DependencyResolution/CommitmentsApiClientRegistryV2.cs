﻿using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.Http;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using StructureMap;
using System.Net.Http;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class CommitmentsApiClientRegistryV2 : Registry
    {
        public CommitmentsApiClientRegistryV2()
        {
            For<EmployerAccountsConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsConfiguration>(ConfigurationKeys.EmployerAccounts)).Singleton();
            For<CommitmentsApiV2ClientConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().CommitmentsApi);

            For<ICommitmentsV2ApiClient>().Use<CommitmentsV2ApiClient>()
                .Ctor<HttpClient>().Is(c => GetHttpV2Client(c));
        }

        private HttpClient GetHttpV2Client(IContext context)
        {
            HttpClient httpClient = new HttpClientBuilder()
                   .WithHandler(new RequestIdMessageRequestHandler())
                   .WithHandler(new SessionIdMessageRequestHandler())
                   .WithDefaultHeaders()
                   .Build();


            return httpClient;
        }
    }    
}
