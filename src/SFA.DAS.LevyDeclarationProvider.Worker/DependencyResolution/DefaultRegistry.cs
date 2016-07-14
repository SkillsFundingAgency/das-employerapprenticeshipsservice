using System;
using System.IO;
using MediatR;
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

        public DefaultRegistry()
        {
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


            //TODO add config service and use Azure service bus queue instead
            For<IPollingMessageReceiver>().Use(() => new Messaging.FileSystem.FileSystemMessageService(@".\Queue"));
            For<ILevyDeclaration>().Use<LevyDeclaration>();
            var rootDir = Path.Combine(Environment.GetEnvironmentVariable("RoleRoot") + @"\", @"approot\TEMP\");
            For<ILevyDeclarationService>().Use<LevyDeclarationFileBasedService>().Ctor<string>().Is(rootDir);

            For<IUserAccountRepository>().Use<UserAccountRepository>().Ctor<string>().Is(@"");
            For<IUserRepository>().Use<FileSystemUserRepository>().Ctor<string>().Is(rootDir);
            For<IEmployerVerificationService>().Use<CompaniesHouseEmployerVerificationService>().Ctor<string>().Is(@"");

            For<IDasLevyRepository>().Use<DasLevyRepository>().Ctor<string>().Is(@"");
            For<IMediator>().Use<Mediator>();
        }

    }

}
