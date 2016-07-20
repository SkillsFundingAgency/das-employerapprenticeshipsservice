using System;
using System.Configuration;
using System.IO;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.LevyAggregationProvider.Worker.Providers;
using SFA.DAS.Messaging;
using StructureMap;

namespace SFA.DAS.LevyAggregationProvider.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.LevyAggregationProvider";
        private const string DevEnv = "LOCAL";

        public DefaultRegistry()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            Scan(
                scan =>
                {
                    scan.WithDefaultConventions();
                    scan.AssemblyContainingType<LevyAggregator>();
                    scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scan.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
                    scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    scan.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                });
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            IConfigurationRepository configurationRepository;

            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }

            var configurationService = new ConfigurationService(
                configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));
            For<IConfigurationService>().Use(configurationService);

            var storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString") ??
                                          "UseDevelopmentStorage=true";

            var config = configurationService.Get<LevyAggregationConfiguration>();

            For<IAggregationRepository>().Use<LevyAggregationRepository>().Ctor<string>().Is(storageConnectionString);
            For<IDasLevyRepository>().Use<DasLevyRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);

            var queueFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            For<IPollingMessageReceiver>().Use(() => new Messaging.FileSystem.FileSystemMessageService(Path.Combine(queueFolder, "RefreshEmployerLevyQueue")));

            For<IMediator>().Use<Mediator>();
        }
    }
}