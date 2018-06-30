﻿using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupEntityFrameworkBehavior(this EndpointConfiguration config)
        {
            config.Pipeline.Register(new UnitOfWorkBehavior(), "Sets up a unit of work for each message");
            config.Pipeline.Register(new UnitOfWorkContextBehavior(), "Sets up a unit of work context for each message");

            return config;
        }
    }
}