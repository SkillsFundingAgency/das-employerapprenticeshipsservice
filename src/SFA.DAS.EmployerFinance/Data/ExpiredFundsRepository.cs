using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;

namespace SFA.DAS.EmployerFinance.Data
{
    public class ExpiredFundsRepository : BaseRepository, IExpiredFundsRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public ExpiredFundsRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task CreateDraft(long accountId, IEnumerable<ExpiredFund> expiredFunds, DateTime now)
        {
            var expiredFundsTable = expiredFunds.ToExpiredFundsDataTable();

            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId);
            parameters.Add("@expiredFunds", expiredFundsTable.AsTableValuedParameter("[employer_financial].[ExpiredFundsTable]"));
            parameters.Add("@now", now);

            await _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[CreateDraftExpiredFunds]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task Create(long accountId, IEnumerable<ExpiredFund> expiredFunds, DateTime now)
        {
            var expiredFundsTable = expiredFunds.ToExpiredFundsDataTable();

            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId);
            parameters.Add("@expiredFunds", expiredFundsTable.AsTableValuedParameter("[employer_financial].[ExpiredFundsTable]"));
            parameters.Add("@now", now);

            await _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[CreateExpiredFunds]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ExpiredFund>> Get(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AccountId", accountId);

            return await _db.Value.Database.Connection.QueryAsync<ExpiredFund>(
                "[employer_financial].[GetExpiredFunds]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<ExpiredFund>> GetDraft(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AccountId", accountId);

            return await _db.Value.Database.Connection.QueryAsync<ExpiredFund>(
                "[employer_financial].[GetDraftExpiredFunds]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
