﻿using SFA.DAS.AutoConfiguration;
using SFA.DAS.EAS.Domain.Configuration;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<EmployerApprenticeshipsServiceConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerApprenticeshipsServiceConfiguration>(ConfigurationKeys.EmployerApprenticeshipsService)).Singleton();
            For<LevyDeclarationProviderConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<LevyDeclarationProviderConfiguration>(ConfigurationKeys.LevyDeclarationProvider)).Singleton();
            For<PaymentProviderConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<PaymentProviderConfiguration>(ConfigurationKeys.PaymentProvider)).Singleton();
            For<EmployerAccountsApiConfiguration>().Use(c => c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().EmployerAccountsApi).Singleton();
        }
    }
}