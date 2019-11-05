using NServiceBus.Persistence.Sql;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class ClientOutboxTransactionExtensions
    {
        public static ISqlStorageSession GetSqlSession(this IClientOutboxTransaction clientOutboxTransaction)
        {
            if (clientOutboxTransaction is ISqlStorageSession sqlSession)
            {
                return sqlSession;
            }

            throw new Exception("Cannot access the SQL session");
        }
    }
}
