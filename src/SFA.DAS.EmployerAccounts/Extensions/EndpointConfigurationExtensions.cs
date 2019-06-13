using System;
using NServiceBus;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NServiceBus.AzureServiceBus;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, Func<string> connectionStringBuilder, IContainer container)
        {
            var isDevelopment = container.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL);

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