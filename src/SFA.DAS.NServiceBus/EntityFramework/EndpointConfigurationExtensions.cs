using System.Data.Entity;
using NServiceBus;
using SFA.DAS.NServiceBus.MsSqlServer;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupEntityFrameworkBehavior<T>(this EndpointConfiguration config) where T : DbContext
        {
            config.Pipeline.Register(new UnitOfWorkBehavior<T>(), "Sets up a unit of work for each message");
            config.Pipeline.Register(new UnitOfWorkContextBehavior(), "Sets up a unit of work context for each message");

            return config;
        }
    }
}