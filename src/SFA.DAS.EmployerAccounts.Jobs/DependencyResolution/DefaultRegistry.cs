using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Jobs.Data;
using StructureMap;
using Microsoft.Extensions.Logging;
using System.Configuration;
using NLog.Extensions.Logging;
using IMembershipRepository = SFA.DAS.EmployerAccounts.Jobs.Data.IMembershipRepository;
using MembershipRepository = SFA.DAS.EmployerAccounts.Jobs.Data.MembershipRepository;

namespace SFA.DAS.EmployerAccounts.Jobs.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<ILoggerFactory>().Use(() => new LoggerFactory().AddApplicationInsights(ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"], null).AddNLog()).Singleton();
            For<ILogger>().Use(c => c.GetInstance<ILoggerFactory>().CreateLogger(c.ParentType));
            For<EmployerAccountsDbContext>().Use(c => new EmployerAccountsDbContext(c.GetInstance<EmployerAccountsConfiguration>().DatabaseConnectionString));
            For<IMembershipRepository>().Use<MembershipRepository>();
            For<IJobHistoryRepository>().Use<JobHistoryRepository>();
        }
    }
}