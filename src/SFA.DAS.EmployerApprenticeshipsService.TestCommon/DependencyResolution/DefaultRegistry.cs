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
using StructureMap.Graph;
using WebGrease.Css.Extensions;
using IConfiguration = SFA.DAS.EAS.Domain.Interfaces.IConfiguration;
using SFA.DAS.HashingService;
using SFA.DAS.EAS.Application.Hashing;

namespace SFA.DAS.EAS.TestCommon.DependencyResolution
{
    public class DefaultRegistry : Registry
    {

        public DefaultRegistry(Mock<IOwinWrapper> owinWrapperMock, Mock<ICookieStorageService<EmployerAccountData>> cookieServiceMock, Mock<IEventsApi> eventApi, Mock<IEmployerCommitmentApi> commitmentsApi)
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
                scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(t => t.Singleton());
            });

            For<IHashingService>().Use(new HashingService.HashingService("12345QWERTYUIOPNDGHAK", "TEST: Dummy hash code London is a city in UK"));

            For<IPublicHashingService>().Use(x => new PublicHashingService("BCDEFGHIJKLMMOPQRSTUVWXYZ", "haShStRiNg"));

            For<IUserRepository>().Use<UserRepository>();

            For<IOwinWrapper>().Use(() => owinWrapperMock.Object);

            For<ICookieStorageService<EmployerAccountData>>().Use(() => cookieServiceMock.Object);
            
            For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();

            For<ICache>().Use<InMemoryCache>();
            For<IEventsApi>().Use(() => eventApi.Object);

            For<ILog>().Use(Mock.Of<ILog>());

            For<IEmployerCommitmentApi>().Use(commitmentsApi.Object);

            For<INotificationsApi>().Use(() => Mock.Of<INotificationsApi>());
            
            AddMediatrRegistrations();

            RegisterMapper();
			RegisterAuditService();
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IMediator>().Use<Mediator>();
        }

        private void RegisterMapper()
        {
            var profiles = Assembly.Load("SFA.DAS.EAS.Infrastructure").GetTypes()
                            .Where(t => typeof(Profile).IsAssignableFrom(t))
                            .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(cfg =>
            {
                profiles.ForEach(cfg.AddProfile);
            });

            var mapper = config.CreateMapper();

            For<IConfigurationProvider>().Use(config).Singleton();
            For<IMapper>().Use(mapper).Singleton();
        }
		
		private void RegisterAuditService()
        {
            For<IAuditApiClient>().Use<StubAuditApiClient>();
        }

    }
}
