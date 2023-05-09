using System;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;

namespace SFA.DAS.EAS.Infrastructure.Extensions;

public static class SynchronizedStorageSessionExtensions
{
    public static ISqlStorageSession GetSqlSession(this SynchronizedStorageSession synchronizedStorageSession)
    {
        if (synchronizedStorageSession is ISqlStorageSession sqlSession)
        {
            return sqlSession;
        }

        throw new InvalidOperationException($"Cannot access the SQL session, type is {synchronizedStorageSession.GetType()}");
    }
}
