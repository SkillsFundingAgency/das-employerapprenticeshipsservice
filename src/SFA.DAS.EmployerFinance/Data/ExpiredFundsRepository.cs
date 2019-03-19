using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;

namespace SFA.DAS.EmployerFinance.Data
{
    public class ExpiredFundsRepository : BaseRepository, IExpiredFundsRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public ExpiredFundsRepository(string connectionString, ILog logger, Lazy<EmployerFinanceDbContext> db) : base(connectionString, logger)
        {
            _db = db;
        }

        public async Task Create(IEnumerable<ExpiredFund> expiredFunds)
        {
            var expiredFundsTable = expiredFunds.ToExpiredFundsDataTable();

            var parameters = new DynamicParameters();

            parameters.Add("@expiredFunds", expiredFundsTable.AsTableValuedParameter("[employer_financial].[ExpiredFundsTable]"));

            await _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[CreateExpiredFunds]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}
