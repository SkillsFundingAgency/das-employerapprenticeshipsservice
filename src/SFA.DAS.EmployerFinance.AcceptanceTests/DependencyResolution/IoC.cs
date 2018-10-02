using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.DependencyResolution;
using SFA.DAS.UnitOfWork.EntityFramework;
using SFA.DAS.UnitOfWork.NServiceBus;
using StructureMap;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<ApprenticeshipLevyRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<CommitmentsRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<EntityFrameworkUnitOfWorkRegistry<EmployerFinanceDbContext>>();
                c.AddRegistry<EventsRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<NotificationsRegistry>();
                c.AddRegistry<NServiceBusUnitOfWorkRegistry>();
                c.AddRegistry<PaymentsRegistry>();
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<DefaultRegistry>();
                c.AddRegistry<MockRegisterations>();
            });
        }
    }
}