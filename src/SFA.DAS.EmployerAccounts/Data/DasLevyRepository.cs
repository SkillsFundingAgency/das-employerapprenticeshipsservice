using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.Levy;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class DasLevyRepository : IDasLevyRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public DasLevyRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
        {
            _db = db;
        }

        public Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(long accountId, string empRef)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@empRef", empRef, DbType.String);

            return _db.Value.Database.Connection.QueryAsync<DasEnglishFraction>(
                sql: "[employer_financial].[GetEnglishFraction_ByEmpRef]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}