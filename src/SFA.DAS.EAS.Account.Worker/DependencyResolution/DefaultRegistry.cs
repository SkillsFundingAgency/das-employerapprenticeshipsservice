using System;
using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.Azure.WebJobs;
using SFA.DAS.EAS.Account.Worker.IdProcessor;
using SFA.DAS.EAS.Account.Worker.Infrastructure;
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
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            var employerApprenticeshipsServiceConfig =
                ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(
                    "SFA.DAS.EmployerApprenticeshipsService");

            Scan(s =>
            {
                s.AssembliesAndExecutablesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
                s.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(c => c.Singleton());
            });

            For<IWebJobConfiguration>().Use(employerApprenticeshipsServiceConfig.WebJobConfig);
            For<JobHost>().Use(ctx => ctx.GetInstance<IJobHostFactory>().CreateJobHost());
        }
    }
}