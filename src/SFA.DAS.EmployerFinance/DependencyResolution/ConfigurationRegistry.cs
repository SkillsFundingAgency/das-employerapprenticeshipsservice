﻿using SFA.DAS.Authorization.EmployerFeatures.Configuration;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Hmrc.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<EmployerAccountsConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsConfiguration>(ConfigurationKeys.EmployerAccounts)).Singleton();
            For<EmployerFinanceConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerFinanceConfiguration>(ConfigurationKeys.EmployerFinance)).Singleton();
            For<ForecastingApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<ForecastingApiClientConfiguration>(ConfigurationKeys.ForecastingApiClient)).Singleton();
            For<IAccountApiConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().AccountApi).Singleton();
            For<EmployerFeaturesConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerFeaturesConfiguration>(ConfigurationKeys.Features)).Singleton();
            For<IHmrcConfiguration>().Use(c => c.GetInstance<EmployerFinanceConfiguration>().Hmrc).Singleton();
        }
    }
}