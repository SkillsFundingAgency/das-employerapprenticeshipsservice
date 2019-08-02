using System;
using NServiceBus;
using SFA.DAS.EmployerFinance.Messages.Commands;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Extensions
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, Func<string> connectionStringBuilder, IContainer container)
        {
            NServiceBus.AzureServiceBus.EndpointConfigurationExtensions.UseAzureServiceBusTransport(config, connectionStringBuilder.Invoke(), r =>
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