using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.Levy;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class DasLevyRepository : IDasLevyRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;
        private readonly LevyDeclarationProviderConfiguration _configuration;

        public DasLevyRepository(LevyDeclarationProviderConfiguration configuration, Lazy<EmployerFinanceDbContext> db)
        {
            _db = db;
            _configuration = configuration;
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