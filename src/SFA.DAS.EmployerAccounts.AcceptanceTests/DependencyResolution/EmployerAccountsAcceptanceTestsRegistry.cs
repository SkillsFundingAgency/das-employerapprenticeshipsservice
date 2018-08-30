using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.NServiceBus.StructureMap;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.DependencyResolution
{
    public class EmployerAccountsAcceptanceTestsRegistry : Registry
    {
        public EmployerAccountsAcceptanceTestsRegistry()
        {
            IncludeRegistry<ApprenticeshipLevyRegistry>();
            IncludeRegistry<AuditRegistry>();
            IncludeRegistry<CachesRegistry>();
            IncludeRegistry<CommitmentsRegistry>();
            IncludeRegistry<ConfigurationRegistry>();
            IncludeRegistry<DataRegistry>();
            IncludeRegistry<SFA.DAS.EmployerFinance.DependencyResolution.ConfigurationRegistry>();
            IncludeRegistry<SFA.DAS.EmployerFinance.DependencyResolution.DataRegistry>();
            IncludeRegistry<EventsRegistry>();
            IncludeRegistry<ExecutionPoliciesRegistry>();
            IncludeRegistry<HashingRegistry>();
            IncludeRegistry<LoggerRegistry>();
            IncludeRegistry<MapperRegistry>();
            IncludeRegistry<MediatorRegistry>();
            IncludeRegistry<MessagePublisherRegistry>();
            IncludeRegistry<NotificationsRegistry>();
            IncludeRegistry<RepositoriesRegistry>();
            IncludeRegistry<PaymentsRegistry>();
            IncludeRegistry<TokenServiceRegistry>();
            IncludeRegistry<NServiceBusRegistry>();

            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<SFA.DAS.EAS.Infrastructure.Data.EmployerAccountsDbContext>().Use(c => new SFA.DAS.EAS.Infrastructure.Data.EmployerAccountsDbContext(c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString));
            For<SFA.DAS.EmployerAccounts.Data.EmployerAccountsDbContext>().Use(c => new SFA.DAS.EmployerAccounts.Data.EmployerAccountsDbContext(c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString));
            For<SFA.DAS.EAS.Infrastructure.Data.EmployerFinanceDbContext>().Use(c => new SFA.DAS.EAS.Infrastructure.Data.EmployerFinanceDbContext(c.GetInstance<EmployerFinanceConfiguration>().DatabaseConnectionString));
            For<SFA.DAS.EmployerFinance.Data.EmployerFinanceDbContext>().Use(c => new SFA.DAS.EmployerFinance.Data.EmployerFinanceDbContext(c.GetInstance<EmployerFinanceConfiguration>().DatabaseConnectionString));
        }
    }
}
