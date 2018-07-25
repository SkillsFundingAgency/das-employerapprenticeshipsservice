using System;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.NServiceBus.AzureServiceBus;
using Environment = SFA.DAS.EmployerAccounts.Configuration.Environment;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupAzureServiceBusTransport(this EndpointConfiguration config, Func<string> connectionStringBuilder)
        {
            var isDevelopment = ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local);

            config.SetupAzureServiceBusTransport(isDevelopment, connectionStringBuilder, r => {});

            return config;
        }
    }
}