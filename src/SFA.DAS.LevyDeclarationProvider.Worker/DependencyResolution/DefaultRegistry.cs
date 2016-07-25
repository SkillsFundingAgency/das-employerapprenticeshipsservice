using System;
using System.IO;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using System.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.DepedencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus;
using StructureMap;
using StructureMap.Graph;

namespace SFA.DAS.LevyDeclarationProvider.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private const string DevEnv = "LOCAL";
        public DefaultRegistry()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(
                    a => a.GetName().Name.StartsWith("SFA.DAS.EmployerApprenticeshipsService") ||
                   a.GetName().Name.StartsWith("SFA.DAS.LevyDeclarationProvider"));
                    
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
                scan.WithDefaultConventions();
                
            });

            For<ILevyDeclarationService>().Use<LevyDeclarationFileBasedService>();
            For<IUserRepository>().Use<FileSystemUserRepository>();

            var configurationRepository = GetConfigurationRepository();

            RegisterMessageQueues(configurationRepository, environment);


            AddMediatrRegistrations();
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository =
                    new AzureTableStorageConfigurationRepository(
                        CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }

        private void RegisterMessageQueues(IConfigurationRepository configurationRepository, string environment)
        {
            var configurationService = new ConfigurationService(
                configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));
            For<IConfigurationService>().Use(configurationService);

            var config = configurationService.Get<EmployerApprenticeshipsServiceConfiguration>();
            if (string.IsNullOrEmpty(config.ServiceBusConnectionString))
            {
                var queueFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                For<IMessagePublisher>().Use(() => new Messaging.FileSystem.FileSystemMessageService(Path.Combine(queueFolder, nameof(QueueNames.das_at_eas_refresh_employer_levy))));
                For<IPollingMessageReceiver>().Use(() => new Messaging.FileSystem.FileSystemMessageService(Path.Combine(queueFolder, nameof(QueueNames.das_at_eas_get_employer_levy))));
            }
            else
            {
                For<IPollingMessageReceiver>().Use(() => new AzureServiceBusMessageService(config.ServiceBusConnectionString, nameof(QueueNames.das_at_eas_get_employer_levy)));
                For<IMessagePublisher>().Use(() => new AzureServiceBusMessageService(config.ServiceBusConnectionString, nameof(QueueNames.das_at_eas_refresh_employer_levy)));
            }
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IMediator>().Use<Mediator>();
        }

    }

}
