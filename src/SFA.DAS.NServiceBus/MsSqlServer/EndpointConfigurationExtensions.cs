using System;
using System.Data.Common;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace SFA.DAS.NServiceBus.MsSqlServer
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupMsSqlServerPersistence(this EndpointConfiguration config, Func<DbConnection> connectionBuilder)
        {
            var persistence = config.UsePersistence<SqlPersistence>();

            persistence.ConnectionBuilder(connectionBuilder);
            persistence.SqlDialect<SqlDialect.MsSqlServer>();

            return config;
        }
    }
}