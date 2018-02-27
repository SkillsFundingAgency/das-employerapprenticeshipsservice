using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Client.Configuration;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using StructureMap;
using WebGrease.Css.Extensions;

namespace SFA.DAS.EAS.LevyDeclarationProvider.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService");

            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<Domain.Interfaces.IConfiguration>().Use<LevyDeclarationProviderConfiguration>();
            For<IEventsApi>().Use<EventsApi>().Ctor<IEventsApiClientConfiguration>().Is(config.EventsApi).SelectConstructor(() => new EventsApi(null));
            For<IUserRepository>().Use<UserRepository>();

            ConfigureHashingService(config);
            RegisterExecutionPolicies();
            RegisterMapper();
            AddMediatrRegistrations();
            RegisterLogger();
        }

        private void RegisterExecutionPolicies()
        {
            For<Infrastructure.ExecutionPolicies.ExecutionPolicy>()
                .Use<Infrastructure.ExecutionPolicies.CompaniesHouseExecutionPolicy>()
                .Named(Infrastructure.ExecutionPolicies.CompaniesHouseExecutionPolicy.Name);

            For<Infrastructure.ExecutionPolicies.ExecutionPolicy>()
                .Use<Infrastructure.ExecutionPolicies.HmrcExecutionPolicy>()
                .Named(Infrastructure.ExecutionPolicies.HmrcExecutionPolicy.Name);

            For<Infrastructure.ExecutionPolicies.ExecutionPolicy>()
                .Use<Infrastructure.ExecutionPolicies.IdamsExecutionPolicy>()
                .Named(Infrastructure.ExecutionPolicies.IdamsExecutionPolicy.Name);
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

        private void RegisterLogger()
        {
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                null,
                null)).AlwaysUnique();
        }

        private void ConfigureHashingService(EmployerApprenticeshipsServiceConfiguration config)
        {
            For<IHashingService>().Use(x => new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring));
        }
    }

}
