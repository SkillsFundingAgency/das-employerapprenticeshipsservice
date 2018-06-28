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

        public static EndpointConfiguration SetupRouting(this EndpointConfiguration config)
        {


            return config;
        }

        public static EndpointConfiguration SetupSendOnly(this EndpointConfiguration config)
        {
            config.SendOnly();

            return config;
        }
    }
}