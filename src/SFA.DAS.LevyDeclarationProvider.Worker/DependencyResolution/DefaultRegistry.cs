using System;
using System.IO;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Messaging;
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
                scan.WithDefaultConventions();

                scan.AssemblyContainingType<GetLevyDeclarationQuery>();
                scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                scan.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
                scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                scan.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>));
            });
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            IConfigurationRepository configurationRepository;



            configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));


            var configurationService = new ConfigurationService(
                configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));
            For<IConfigurationService>().Use(configurationService);

            var config = configurationService.Get<EmployerApprenticeshipsServiceConfiguration>();

            //TODO add config service and use Azure service bus queue instead
            For<IPollingMessageReceiver>().Use(() => new Messaging.FileSystem.FileSystemMessageService(@".\GetEmployerLevyQueue"));
            For<IMessagePublisher>().Use(() => new Messaging.FileSystem.FileSystemMessageService(@".\RefreshEmployerLevyQueue"));
            For<ILevyDeclaration>().Use<LevyDeclaration>();
            var rootDir = Path.Combine(Environment.GetEnvironmentVariable("RoleRoot") + @"\", @"approot\App_Data\");
            For<ILevyDeclarationService>().Use<LevyDeclarationFileBasedService>().Ctor<string>().Is(rootDir);

            For<IUserAccountRepository>().Use<UserAccountRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);
            For<IUserRepository>().Use<FileSystemUserRepository>().Ctor<string>().Is(rootDir);
            For<IEmployerVerificationService>().Use<CompaniesHouseEmployerVerificationService>().Ctor<string>().Is(config.CompaniesHouse.ApiKey);
            For<IEmployerAccountRepository>().Use<EmployerAccountRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);
            For<IEmployerSchemesRepository>().Use<EmployerSchemesRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);
            For<IDasLevyRepository>().Use<DasLevyRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);

            For<IMediator>().Use<Mediator>();
        }

    }

}
