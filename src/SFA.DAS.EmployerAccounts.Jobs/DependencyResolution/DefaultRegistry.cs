using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Jobs.Data;
using StructureMap;
using Microsoft.Extensions.Logging;
using System.Configuration;
using NLog.Extensions.Logging;

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
            For<EmployerAccountsDbContext>().Use(c => new EmployerAccountsDbContext(c.GetInstance<EmployerAccountsConfiguration>().DatabaseConnectionString));
            For<IPopulateRepository>().Use< PopulateRepository>();
        }
    }
}