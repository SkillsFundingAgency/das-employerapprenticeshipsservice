using System;
using NServiceBus;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NServiceBus.AzureServiceBus;
using Environment = SFA.DAS.Configuration.Environment;

namespace SFA.DAS.EAS.Infrastructure.NServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, Func<string> connectionStringBuilder)
        {
            var isDevelopment = ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local);

            config.UseAzureServiceBusTransport(isDevelopment, connectionStringBuilder, r =>
            {
                r.RouteToEndpoint(
                    typeof(ImportLevyDeclarationsCommand).Assembly,
                    typeof(ImportLevyDeclarationsCommand).Namespace,
                    "SFA.DAS.EmployerFinance.MessageHandlers");
            });

            return config;
        }
    }
}