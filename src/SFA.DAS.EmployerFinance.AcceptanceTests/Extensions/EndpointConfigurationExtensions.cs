using System;
using System.Text.RegularExpressions;
using Microsoft.Azure;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NServiceBus.AzureServiceBus;
using Environment = SFA.DAS.Configuration.Environment;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config)
        {
            // It is important that we use the LearningTransport here to avoid clashes with the real message handler web job (which will be running in the AT env).
            config.UseAzureServiceBusTransport(null, r =>
            {
                r.RouteToEndpoint(
                    typeof(ImportLevyDeclarationsCommand).Assembly,
                    typeof(ImportLevyDeclarationsCommand).Namespace,
                    "SFA.DAS.EmployerFinance.AcceptanceTests");
            },
                TrySetSetStorageFolder);

            return config;
        }

        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config,
            Func<string> connectionStringBuilder, 
            Action<RoutingSettings> routing,
            Action<TransportExtensions<LearningTransport>> transportSettings)
        {
            TransportExtensions<LearningTransport> config1 = config.UseTransport<LearningTransport>();
            config1.Transactions(TransportTransactionMode.ReceiveOnly);
            routing((RoutingSettings)config1.Routing<LearningTransport>());
            transportSettings?.Invoke(config1);
            return config;
        }

        private static void TrySetSetStorageFolder(TransportExtensions<LearningTransport> settings)
        {
            if (TryGetStorageFolder(out var storageFolder))
            {
                settings.StorageDirectory(storageFolder);
            }
        }

        private static bool TryGetTransportSettings(EndpointConfiguration config, out TransportExtensions<LearningTransport> transportSettings)
        {
            transportSettings =
                config.GetSettings().Get<TransportExtensions>() as TransportExtensions<LearningTransport>;

            return transportSettings != null;
        }

        private static bool TryGetStorageFolder(out string storageFolder)
        {
            storageFolder = CloudConfigurationManager.GetSetting("LearningTransportFolder");

            if (!string.IsNullOrWhiteSpace(storageFolder))
            {
                var m = Regex.Match(storageFolder, @"\$\((.*)\)", RegexOptions.None);
                if (m.Groups.Count > 0)
                {
                    var envVariable = m.Groups[1];

                    var envVariableName = envVariable.Value.ConvertVSTSVariableToEnvName();

                    storageFolder = System.Environment.GetEnvironmentVariable(envVariableName);
                }
            }

            return !string.IsNullOrWhiteSpace(storageFolder);
        }

        private static string ConvertVSTSVariableToEnvName(this string vstsVariableName)
        {
            return vstsVariableName.ToUpperInvariant().Replace('.', '_');
        }
    }
}