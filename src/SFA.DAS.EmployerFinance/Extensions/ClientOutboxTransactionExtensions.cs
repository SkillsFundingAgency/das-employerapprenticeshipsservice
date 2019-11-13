﻿using NServiceBus.Persistence.Sql;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using System;

namespace SFA.DAS.EmployerFinance.Extensions
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
