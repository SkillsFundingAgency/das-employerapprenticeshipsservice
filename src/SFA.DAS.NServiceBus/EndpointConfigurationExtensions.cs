using NServiceBus;

namespace SFA.DAS.NServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupErrorQueue(this EndpointConfiguration config)
        {
            config.SendFailedMessagesTo("errors");

            return config;
        }

        public static EndpointConfiguration SetupInstallers(this EndpointConfiguration config)
        {
            config.EnableInstallers();

            return config;
        }

        public static EndpointConfiguration SetupOutbox(this EndpointConfiguration config)
        {
            config.EnableOutbox();

            return config;
        }

        public static EndpointConfiguration SetupPurgeOnStartup(this EndpointConfiguration config)
        {
            config.PurgeOnStartup(true);

            return config;
        }

        public static EndpointConfiguration SetupRouting(this EndpointConfiguration config)
        {


            return config;
        }

        public static EndpointConfiguration SetupSendOnly(this EndpointConfiguration config)
        {
            config.SendOnly();

            return config;
        }

        public static EndpointConfiguration SetupUnitOfWork(this EndpointConfiguration config)
        {
            config.RegisterComponents(c =>
            {
                if (!c.HasComponent<IDb>())
                {
                    c.ConfigureComponent<Db>(DependencyLifecycle.InstancePerUnitOfWork);
                }
            });

            config.Pipeline.Register(new UnitOfWorkBehavior(), "Sets up a unit of work for each message");

            return config;
        }
    }
}