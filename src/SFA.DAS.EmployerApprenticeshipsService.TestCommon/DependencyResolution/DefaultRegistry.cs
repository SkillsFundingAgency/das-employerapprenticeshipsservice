using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using Moq;
using SFA.DAS.Audit.Client;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using StructureMap;
using WebGrease.Css.Extensions;
using IConfiguration = SFA.DAS.EAS.Domain.Interfaces.IConfiguration;
using SFA.DAS.HashingService;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Infrastructure.Authentication;
using StructureMap.TypeRules;

namespace SFA.DAS.EAS.TestCommon.DependencyResolution
{
    public class DefaultRegistry : Registry
    {

        public DefaultRegistry(Mock<IAuthenticationService> owinWrapperMock, Mock<ICookieStorageService<EmployerAccountData>> cookieServiceMock, Mock<IEventsApi> eventApi, Mock<IEmployerCommitmentApi> commitmentsApi)
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
                s.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(t => t.Singleton());
            });

            For<IAuthenticationService>().Use(() => owinWrapperMock.Object);
            For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();
            For<ICookieStorageService<EmployerAccountData>>().Use(() => cookieServiceMock.Object);
            For<IEmployerCommitmentApi>().Use(commitmentsApi.Object);
            For<IEventsApi>().Use(() => eventApi.Object);
            For<IHashingService>().Use(new HashingService.HashingService("12345QWERTYUIOPNDGHAK", "TEST: Dummy hash code London is a city in UK"));
            For<ILog>().Use(Mock.Of<ILog>());
            For<INotificationsApi>().Use(() => Mock.Of<INotificationsApi>());
            For<IPublicHashingService>().Use(x => new PublicHashingService("BCDEFGHIJKLMMOPQRSTUVWXYZ", "haShStRiNg"));
            For<IUserRepository>().Use<UserRepository>();

            RegisterMapper();
            RegisterMediator();
			RegisterAuditService();
        }

        private void RegisterMapper()
        {
            var profiles = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => a.FullName.StartsWith("SFA.DAS.EAS"))
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(Profile).IsAssignableFrom(t) && t.IsConcrete() && t.HasConstructors())
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
            });

            var mapper = config.CreateMapper();

            For<IConfigurationProvider>().Use(config).Singleton();
            For<IMapper>().Use(mapper).Singleton();
        }

        private void RegisterMediator()
        {
            For<IMediator>().Use<Mediator>();
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
        }

        private void RegisterAuditService()
        {
            For<IAuditApiClient>().Use<StubAuditApiClient>();
        }

    }
}
