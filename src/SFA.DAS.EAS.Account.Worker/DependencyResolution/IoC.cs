using SFA.DAS.EAS.Application.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Account.Worker.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<AuditRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<CommitmentsRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LevyRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<NotificationsRegistry>();
                c.AddRegistry<PaymentsRegistry>();
                c.AddRegistry<ReferenceDataRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<ValidationRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}