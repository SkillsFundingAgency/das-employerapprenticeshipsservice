using System;
using NServiceBus;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NServiceBus.AzureServiceBus;
using StructureMap;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, Func<string> connectionStringBuilder)
        {
            var isDevelopment = ConfigurationHelper.IsEnvironmentAnyOf(DAS.Configuration.Environment.Local);

            config.UseAzureServiceBusTransport(isDevelopment, connectionStringBuilder, r =>
            {
                r.RouteToEndpoint(
                    typeof(ImportLevyDeclarationsCommand).Assembly,
                    typeof(ImportLevyDeclarationsCommand).Namespace,
                    "SFA.DAS.EmployerFinance.AcceptanceTests.Steps.Jobs");
            });

            return config;
        }

        public static IEndpointInstance UseEndpoint(this IEndpointInstance endpointInstance,
            IContainer container)
        {
            container.Configure(c => { c.For<IEndpointInstance>().Use(endpointInstance); });
            container.Configure(c => { c.For<IMessageSession>().Use(endpointInstance); });

            return endpointInstance;
        }
    }
}