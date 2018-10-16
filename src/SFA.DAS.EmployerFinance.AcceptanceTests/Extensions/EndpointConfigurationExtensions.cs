using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Azure;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus.AzureServiceBus;
using Environment = SFA.DAS.Configuration.Environment;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class EndpointConfigurationExtensions
    {

        private static readonly ILog _log = new NLogLogger(typeof(EndpointConfigurationExtensions));




        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config,
            Func<string> connectionStringBuilder)
        {
            var isDevelopment = ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local);

            // If we use SendLocal we can use the configured queue
            config.UseAzureServiceBusTransport(isDevelopment, connectionStringBuilder, r =>
            {
                r.RouteToEndpoint(
                    typeof(ImportLevyDeclarationsCommand).Assembly,
                    typeof(ImportLevyDeclarationsCommand).Namespace,
                    "SFA.DAS.EmployerFinance.AcceptanceTests");
            });

            return config;
        }


        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config)
        {
            _log.Info("Setting NService bus to end point config");
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
            transportSettings?.Invoke(config1);

            config1.Transactions(TransportTransactionMode.ReceiveOnly);
            routing((RoutingSettings) config1.Routing<LearningTransport>());
            return config;
        }

        private static void TrySetSetStorageFolder(TransportExtensions<LearningTransport> settings)
        {
            // The learning transport requires a folder name, which defaults to the sln folder. When running on the build
            // agent there is no sln folder, so learning transport will throw an exception. A folder can be set using
            // the .StorageDirectory() extension so we'll set this from config. We won't know exactly what the folder 
            // will be so instead we'll allow the config value to be formatted as $(EnvVariableName) so that we can 
            // use a build agent variable to specify the folder.
            if (TryGetStorageFolder(out var storageFolder))
            {
                storageFolder = Path.Combine(storageFolder, $".learningtransport.{Guid.NewGuid()}");
                _log.Debug($"Using folder {storageFolder} for learning transport storage folder");
                settings.StorageDirectory(storageFolder);
            }
        }

        private static bool TryGetStorageFolder(out string storageFolder)
        {
            storageFolder = CloudConfigurationManager.GetSetting("LearningTransportFolder");

            if (TryToInterpretConfigValueAsEnvVariable(storageFolder, out string envVariableName))
            {
                storageFolder = System.Environment.GetEnvironmentVariable(envVariableName);
                _log.Debug($"Environment variable '{envVariableName}' resolved to folder name '{storageFolder}'");

                if (string.IsNullOrWhiteSpace(storageFolder))
                {
                    LogEnvironmentVariables();
                }
            }

            _log.Debug($"Resolved folder {storageFolder} for learning transport storage folder");

            return !string.IsNullOrWhiteSpace(storageFolder);
        }

        public static string ConvertVSTSVariableToEnvName(this string vstsVariableName)
        {
            return vstsVariableName.ToUpperInvariant().Replace('.', '_');
        }

        public static bool TryToInterpretConfigValueAsEnvVariable(string configVariableName, out string envVariableName)
        {
            envVariableName = null;

            if (!string.IsNullOrWhiteSpace(configVariableName))
            {
                var m = Regex.Match(configVariableName, @"\$\((.*)\)");

                if (m.Groups.Count > 1)
                { 
                    var envVariable = m.Groups[1];

                    envVariableName = envVariable.Value.ConvertVSTSVariableToEnvName();

                    _log.Debug($"Converted config value '{configVariableName}' to '{envVariableName}'");
                }
            }

            return !string.IsNullOrWhiteSpace(envVariableName);
        }

        private static void LogEnvironmentVariables()
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                "The variable name does not exist as an environment variable - the following environment variables exist");

            foreach (DictionaryEntry env in System.Environment.GetEnvironmentVariables())
            {
                sb.Append(env.Key);
                sb.Append('=');
                sb.AppendLine(env.Value.ToString());
            }

            _log.Debug(sb.ToString());
        }
    }
}