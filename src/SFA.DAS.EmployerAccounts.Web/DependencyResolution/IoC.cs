

namespace SFA.DAS.EmployerAccounts.Web.DependencyResolution
{
    using SFA.DAS.EmployerAccounts.DependencyResolution;
    using StructureMap;

    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry<AuthorizationRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<EventsRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<NotificationsRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
