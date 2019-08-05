using System;
using NServiceBus;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerFinance.Messages.Commands;
using StructureMap;


namespace SFA.DAS.EAS.Web.Extensions
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, Func<string> connectionStringBuilder, IContainer container)
        {
            var isDevelopment = container.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL);

            var settings = new Action<RoutingSettings>(
                r => r.RouteToEndpoint(
                        typeof(ImportLevyDeclarationsCommand).Assembly,
                        typeof(ImportLevyDeclarationsCommand).Namespace,
                        "SFA.DAS.EmployerFinance.MessageHandlers"));

            if (isDevelopment)
            {
                NServiceBus.EndpointConfigurationExtensions.UseLearningTransport(config, settings);
            }
            else
            {
                NServiceBus.AzureServiceBus.EndpointConfigurationExtensions.UseAzureServiceBusTransport(config, connectionStringBuilder.Invoke(), settings);
            }

            return config;
        }
    }
}