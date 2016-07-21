using System;
using System.IO;
using System.Linq;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus;
using StructureMap.Configuration.DSL;
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
                    a => a.GetName().Name.StartsWith("SFA.DAS.EmployerApprenticeshipsService")
                         && !a.GetName().Name.Equals("SFA.DAS.EmployerApprenticeshipsService.Infrastructure"));
                    
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });
          
            IConfigurationRepository configurationRepository;

            configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));

            var configurationService = new ConfigurationService(
                configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));
            For<IConfigurationService>().Use(configurationService);

            var config = configurationService.Get<EmployerApprenticeshipsServiceConfiguration>();
                        var storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString") ??
                                          "UseDevelopmentStorage=true";

            if (string.IsNullOrEmpty(config.ServiceBusConnectionString))
            {
                var queueFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                For<IPollingMessageReceiver>().Use(() => new Messaging.FileSystem.FileSystemMessageService(Path.Combine(queueFolder, nameof(QueueNames.das_at_eas_get_employer_levy))));
                For<IMessagePublisher>().Use(() => new Messaging.FileSystem.FileSystemMessageService(Path.Combine(queueFolder, nameof(QueueNames.das_at_eas_refresh_employer_levy))));
            }
            else
            {
                For<IPollingMessageReceiver>().Use(() => new AzureServiceBusMessageService(config.ServiceBusConnectionString, nameof(QueueNames.das_at_eas_get_employer_levy)));
                For<IMessagePublisher>().Use(() => new AzureServiceBusMessageService(config.ServiceBusConnectionString, nameof(QueueNames.das_at_eas_refresh_employer_levy)));
            }

            For<ILevyDeclaration>().Use<LevyDeclaration>();
            var rootDir = Path.Combine(Environment.GetEnvironmentVariable("RoleRoot") + @"\", @"approot\App_Data\");
            For<ILevyDeclarationService>().Use<LevyDeclarationFileBasedService>().Ctor<string>().Is(rootDir);
            For<IAggregationRepository>().Use<LevyAggregationRepository>().Ctor<string>().Is(storageConnectionString);
            For<IUserAccountRepository>().Use<UserAccountRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);
            For<IAccountRepository>().Use<AccountRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);
            For<IUserRepository>().Use<FileSystemUserRepository>().Ctor<string>().Is(rootDir);
            For<IEmployerVerificationService>().Use<CompaniesHouseEmployerVerificationService>().Ctor<string>().Is(config.CompaniesHouse.ApiKey);
            For<IEmployerAccountRepository>().Use<EmployerAccountRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);
            For<IEmployerSchemesRepository>().Use<EmployerSchemesRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);
            For<IDasLevyRepository>().Use<DasLevyRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);

            AddMediatrRegistrations();
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IMediator>().Use<Mediator>();
        }

    }

}
