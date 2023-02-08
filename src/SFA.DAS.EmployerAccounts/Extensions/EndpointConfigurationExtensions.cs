using System;
using NServiceBus;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, Func<string> connectionStringBuilder, IContainer container)
        {
            var isDevelopment = container.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL);

            if (isDevelopment)
            {
                var transport = config.UseTransport<LearningTransport>();
                transport.Transactions(TransportTransactionMode.ReceiveOnly);
                ConfigureRouting(transport.Routing());
            }

            else
            {
                config.UseAzureServiceBusTransport(connectionStringBuilder(), ConfigureRouting);
            }

            // 5 seperate endpoints call this helper method, easier to add here.
            config.UseMessageConventions();

            return config;
        }

        private static void ConfigureRouting(RoutingSettings routing)
        {
            routing.RouteToEndpoint(
                    typeof(ImportLevyDeclarationsCommand).Assembly,
                    typeof(ImportLevyDeclarationsCommand).Namespace,
                    "SFA.DAS.EmployerFinance.MessageHandlers"
            );

            routing.RouteToEndpoint(
                typeof(SendEmailCommand).Assembly,
                typeof(SendEmailCommand).Namespace,
                "SFA.DAS.Notifications.MessageHandlers"
            );
        }
    }
}