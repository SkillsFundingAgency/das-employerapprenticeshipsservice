using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap;
using SFA.DAS.EmployerAccounts.DependencyResolution;
using SFA.DAS.UnitOfWork.EntityFramework;
using SFA.DAS.UnitOfWork.NServiceBus;
using SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox;
using StructureMap;
using SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap;
using ConfigurationRegistry = SFA.DAS.EmployerAccounts.DependencyResolution.ConfigurationRegistry;

namespace SFA.DAS.EmployerAccounts.Web.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<ActivitiesClientRegistry>();
                c.AddRegistry<ApprenticeshipLevyRegistry>();
                c.AddRegistry<AuditRegistry>();
                c.AddRegistry<AuthorizationRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<DateTimeRegistry>();
                c.AddRegistry<CommitmentsRegistry>();
                c.AddRegistry<EmployerAccountsApiClientRegistry>();
                c.AddRegistry<AccountApiClientRegistry>();
                c.AddRegistry<EntityFrameworkUnitOfWorkRegistry<EmployerAccountsDbContext>>();
                c.AddRegistry<EventsRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<HmrcRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<NotificationsRegistry>();
                c.AddRegistry<NServiceBusClientUnitOfWorkRegistry>();
                c.AddRegistry<NServiceBusUnitOfWorkRegistry>();
                c.AddRegistry<ReferenceDataRegistry>();
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry<ServicesRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<TasksRegistry>();
                c.AddRegistry<ValidationRegistry>();
                c.AddRegistry<PortalClientRegistry>();
                c.AddRegistry<PensionsRegulatorRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}