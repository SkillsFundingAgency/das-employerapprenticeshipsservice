using System;
using NServiceBus;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NServiceBus.AzureServiceBus;
using Environment = SFA.DAS.Configuration.Environment;

namespace SFA.DAS.EmployerFinance.Extensions
{
    public static class EndpointConfigurationExtensions
    {
        private const string ApplicationNameSpace = "SFA.DAS.EmployerFinance";

        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config,
            Func<string> connectionStringBuilder)
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

        public static EndpointConfiguration UseDefaultLocationsForAllMessages(this EndpointConfiguration config, string applicationNameSpace)
        {
            return config
                    .UseDefaultLocationsForAllCommands(applicationNameSpace)
                    .UseDefaultLocationsForAllEvents(applicationNameSpace);
        }

        public static EndpointConfiguration UseDefaultLocationsForAllCommands(this EndpointConfiguration config, string applicationNameSpace)
        {
            var conventions = config.Conventions();
            conventions.DefiningCommandsAs(IsCommand);
            return config;
        }

        public static EndpointConfiguration UseDefaultLocationsForAllEvents(this EndpointConfiguration config, string applicationNameSpace)
        {
            var conventions = config.Conventions();
            conventions.DefiningEventsAs(IsEvent);
            return config;
        }

        private static bool IsCommand(Type type)
        {
            return string.Equals(type.Namespace,  $"{ApplicationNameSpace}.MessageHandlers.Commands", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsEvent(Type type)
        {
            return string.Equals(type.Namespace, $"{ApplicationNameSpace}.MessageHandlers.Events", StringComparison.OrdinalIgnoreCase);
        }
    }
}