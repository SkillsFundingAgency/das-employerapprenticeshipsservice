﻿using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Infrastructure.Data;
using StructureMap;
using StructureMap.Graph;
using WebGrease.Css.Extensions;
using IConfiguration = SFA.DAS.EAS.Domain.Interfaces.IConfiguration;

namespace SFA.DAS.EAS.LevyDeclarationProvider.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<IUserRepository>().Use<UserRepository>();

            For<IConfiguration>().Use<LevyDeclarationProviderConfiguration>();
            
            RegisterExecutionPolicies();

            RegisterMapper();

            AddMediatrRegistrations();
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

    }

}
