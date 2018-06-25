using NServiceBus;
using NServiceBus.InMemory.Outbox;
using NServiceBus.Persistence.Sql;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Messages.Events;
using StructureMap;
using System;
using System.Data.SqlClient;

namespace SFA.DAS.EAS.Infrastructure.NServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupAzureTransport(
            this EndpointConfiguration config,
            string connectionString)
        {
            var transport = config.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(connectionString);
            transport.UseForwardingTopology();
            transport.Transactions(TransportTransactionMode.ReceiveOnly);

            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(CreatedAccountEvent).Assembly, "Commands", "MessageHandlers");

            return config;
        }

        public static EndpointConfiguration SetupOutbox(
            this EndpointConfiguration config)
        {
            var outboxSettings = config.EnableOutbox();
            outboxSettings.KeepDeduplicationDataFor(TimeSpan.FromDays(6));
            outboxSettings.TimeToKeepDeduplicationData(TimeSpan.FromMinutes(15));

            return config;
        }

        public static EndpointConfiguration SetupSqlPersistance(
            this EndpointConfiguration config,
            string databaseConnectionString)
        {
            var persistence = config.UsePersistence<SqlPersistence>();

            persistence.ConnectionBuilder(() => new SqlConnection(databaseConnectionString));

            return config;
        }

        public static EndpointConfiguration SetupHeartbeat(
            this EndpointConfiguration config)
        {
            config.SendHeartbeatTo(
                serviceControlQueue: "heartbeat",
                frequency: TimeSpan.FromSeconds(10),
                timeToLive: TimeSpan.FromSeconds(20));

            return config;
        }

        public static EndpointConfiguration SetupMetrics(
            this EndpointConfiguration config)
        {
            var metrics = config.EnableMetrics();

            metrics.SendMetricDataToServiceControl(
                serviceControlMetricsAddress: "metrics",
                interval: TimeSpan.FromSeconds(10),
                instanceId: "INSTANCE_ID_OPTIONAL");

            return config;
        }

        public static EndpointConfiguration Setup(this EndpointConfiguration config, IContainer container, bool isDevelopment)
        {
            config.EnableInstallers();
            config.UseContainer<StructureMapBuilder>(customizations => customizations.ExistingContainer(container));

            if (isDevelopment)
            {
                config.UseTransport<LearningTransport>();
            }
            else
            {
                var configuration = container.GetInstance<EmployerApprenticeshipsServiceConfiguration>();

                config.SetupAzureTransport(configuration.MessageServiceBusConnectionString)
                    .SetupOutbox()
                    .SetupSqlPersistance(configuration.DatabaseConnectionString);
            }

            config.SendFailedMessagesTo("errors");
            config.UseSerialization<NewtonsoftSerializer>();

            return config;
        }
    }
}
