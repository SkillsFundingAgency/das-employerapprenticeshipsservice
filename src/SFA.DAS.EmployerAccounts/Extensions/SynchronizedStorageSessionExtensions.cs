using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class SynchronizedStorageSessionExtensions
    {
        public static ISqlStorageSession GetSqlSession(this SynchronizedStorageSession synchronizedStorageSession)
        {
            if (synchronizedStorageSession is ISqlStorageSession sqlSession)
            {
                return sqlSession;
            }

            throw new Exception("Cannot access the SQL session");
        }
    }
}
