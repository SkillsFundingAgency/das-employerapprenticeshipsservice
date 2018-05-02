using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EAS.PaymentProvider.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<Domain.Interfaces.IConfiguration>().Use<PaymentProviderConfiguration>();

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
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IMediator>().Use<Mediator>();
        }

        private void RegisterMapper()
        {
            var profiles = Assembly.Load("SFA.DAS.EAS.Infrastructure")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t)).ToList();

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
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
    }
}