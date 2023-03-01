using NServiceBus;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;

namespace SFA.DAS.EmployerAccounts.Extensions;

public static class EndpointConfigurationExtensions
{
    public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, Func<string> connectionStringBuilder, bool isLocal)
    {
        if (isLocal)
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