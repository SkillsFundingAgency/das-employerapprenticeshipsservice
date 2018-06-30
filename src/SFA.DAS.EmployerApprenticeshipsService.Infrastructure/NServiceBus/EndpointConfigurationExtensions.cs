using System;
using NServiceBus;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Messages.Commands;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using Environment = SFA.DAS.EAS.Infrastructure.DependencyResolution.Environment;

namespace SFA.DAS.EAS.Infrastructure.NServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupAzureServiceBusTransport(this EndpointConfiguration config, Func<string> connectionStringBuilder)
        {
            var isDevelopment = ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local);

            config.SetupAzureServiceBusTransport(isDevelopment, connectionStringBuilder, r =>
            {
                r.RouteToEndpoint(
                    typeof(ImportLevyDeclarationsCommand).Assembly,
                    typeof(ImportLevyDeclarationsCommand).Namespace,
                    "SFA.DAS.EAS.MessageHandlers");

                r.RouteToEndpoint(
                    typeof(ProcessOutboxMessageCommand),
                    "SFA.DAS.EAS.MessageHandlers");
            });

            return config;
        }
    }
}