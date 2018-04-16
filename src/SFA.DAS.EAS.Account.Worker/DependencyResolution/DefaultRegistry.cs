using System;
using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.Azure.WebJobs;
using SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.NLog.Logger;
using StructureMap;
using StructureMap.TypeRules;

namespace SFA.DAS.EAS.Account.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            var employerApprenticeshipsServiceConfig = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService");
	        var levyAggregationProviderConfig = ConfigurationHelper.GetConfiguration<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider");
	        var commitmentsAPIConfig = ConfigurationHelper.GetConfiguration<CommitmentsApiClientConfiguration>("SFA.DAS.CommitmentsAPI");
	        var paymentsAPIConfig = ConfigurationHelper.GetConfiguration<PaymentsApiClientConfiguration>("SFA.DAS.PaymentsAPI");

			Scan(s =>
            {
                s.AssembliesAndExecutablesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
                s.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(c => c.Singleton());
            });

            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
            For<IPublicHashingService>().Use(() => new PublicHashingService(employerApprenticeshipsServiceConfig.PublicAllowedHashstringCharacters, employerApprenticeshipsServiceConfig.PublicHashstring));
            For<ICache>().Use<InMemoryCache>();
			For<EmployerApprenticeshipsServiceConfiguration>().Use(employerApprenticeshipsServiceConfig);
	        For<IWebJobConfiguration>().Use(employerApprenticeshipsServiceConfig.WebJobConfig);
	        For<LevyDeclarationProviderConfiguration>().Use(levyAggregationProviderConfig);
	        For<CommitmentsApiClientConfiguration>().Use(commitmentsAPIConfig);
	        For<PaymentsApiClientConfiguration>().Use(paymentsAPIConfig);
	        For<JobHost>().Use(ctx => ctx.GetInstance<IJobHostFactory>().CreateJobHost());

			RegisterMediator();

            RegisterMapper();
        }
        
        private void RegisterMediator()
        {
            For<IMediator>().Use<Mediator>();
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
	}

        private void RegisterMapper()
        {
            var profiles = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => a.FullName.StartsWith("SFA.DAS.EAS"))
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(Profile).IsAssignableFrom(t) && t.IsConcrete() && t.HasConstructors())
                .Select(t => (Profile)Activator.CreateInstance(t));

            Mapper.Initialize(c =>
            {
                foreach (var profile in profiles)
                {
                    c.AddProfile(profile);
                }
            });

            For<IConfigurationProvider>().Use(Mapper.Configuration).Singleton();
            For<IMapper>().Use(Mapper.Instance).Singleton();
        }
    }
}