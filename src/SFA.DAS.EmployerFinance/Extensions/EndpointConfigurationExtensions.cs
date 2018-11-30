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
                    .UseDefaultLocationsForAllMessages(applicationNameSpace);
        }

        public static EndpointConfiguration UseDefaultLocationsForAllCommands(this EndpointConfiguration config, string applicationNameSpace)
        {
            var conventions = config.Conventions();

            return config;
        }

        public static EndpointConfiguration UseDefaultLocationsForAllEvents(this EndpointConfiguration config, string applicationNameSpace)
        {
            var conventions = config.Conventions();

            return config;
        }

        private static string GetApplicationMessageNameSpace(string applicationNameSpace)
        {
            // Our convention is for solutions to be named SFA.DAS.app-name and projects within that solution to be named SFA.DAS.app-name.project-name
            // where project-name is something like Jobs, MessageHandlers, API and Web. Messages are stored in a project named SFA.DAS.app-name.Messages.
            // So for application name space "SFA.DAS.EmployerFinance.xxxx" return "SFA.DAS.EmployerFinance.Messages"

            // name-spaces are optional in .net
            if (string.IsNullOrWhiteSpace(applicationNameSpace))
            {
                return string.Empty;
            }

            var indexOfThirdWord = applicationNameSpace. IndexOf('.');

            if (indexOfThirdWord < 1)
            {
                return String.Empty;
            }

            return applicationNameSpace.Substring(0, indexOfThirdWord);
        }
    }
}