using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.DependencyResolution;
using SFA.DAS.UnitOfWork.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.EntityFramework.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.StructureMap;
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
                c.AddRegistry<CommitmentsV2ApiClientRegistry>();
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
                c.AddRegistry<MocksRegistry>();
                c.AddRegistry<NotificationsRegistry>();
                c.AddRegistry<NServiceBusClientUnitOfWorkRegistry>();
                c.AddRegistry<NServiceBusUnitOfWorkRegistry>();
                c.AddRegistry<PaymentsRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<UnitOfWorkRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}