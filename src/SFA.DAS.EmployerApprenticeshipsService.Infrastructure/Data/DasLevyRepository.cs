using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class DasLevyRepository : BaseRepository, IDasLevyRepository
    {
        private readonly LevyDeclarationProviderConfiguration _configuration;
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public DasLevyRepository(LevyDeclarationProviderConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _configuration = configuration;
            _db = db;
        }

        public async Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<LevyDeclarationView>(
                sql: "[employer_financial].[GetLevyDeclarations_ByAccountId]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId, string payrollYear, short payrollMonth)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@payrollYear", payrollYear, DbType.String);
            parameters.Add("@payrollMonth", payrollMonth, DbType.Int16);

            var result = await _db.Value.Database.Connection.QueryAsync<LevyDeclarationView>(
                sql: "[employer_financial].[GetLevyDeclarations_ByAccountPayrollMonthPayrollYear]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds)
        {
            var accountParametersTable = new AccountIdUserTableParam(accountIds);

            accountParametersTable.Add("@allowancePercentage", _configuration.TransferAllowancePercentage, DbType.Decimal);

            var result = await _db.Value.Database.Connection.QueryAsync<AccountBalance>(
                sql: "[employer_financial].[GetAccountBalance_ByAccountIds]",
                param: accountParametersTable,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
    }
}