using NServiceBus;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;

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
                transport.Routing().AddRouting();
            }
            else
            {
                config.UseAzureServiceBusTransport(connectionStringBuilder(), s=> s.AddRouting());
            }

            // 5 separate endpoints call this helper method, easier to add here.
            config.UseMessageConventions();

            return config;
        }
    }
}