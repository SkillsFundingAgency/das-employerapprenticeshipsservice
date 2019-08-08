using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _accountDb;
        private readonly Lazy<EmployerAccountsDbContext> _financeDb;

        public AccountRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> accountDb, Lazy<EmployerAccountsDbContext> financeDb)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _accountDb = accountDb;
        }

        public async Task<string> GetAccountName(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);

            var result = await _accountDb.Value.Database.Connection.QueryAsync<string>(
                sql: "SELECT Name FROM [employer_account].[Account] WHERE Id = @accountId",
                param: parameters,
                transaction: _accountDb.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public async Task<Dictionary<long, string>> GetAccountNames(IEnumerable<long> accountIds)
        {
            var result = await _accountDb.Value.Database.Connection.QueryAsync<AccountNameItem>(
                sql: "SELECT Id, Name FROM [employer_account].[Account] WHERE Id IN @accountIds",
                param: new { accountIds = accountIds },
                transaction: _accountDb.Value.Database.CurrentTransaction.UnderlyingTransaction);

            return result.ToDictionary(d => d.Id, d => d.Name);
        }

        public async Task CreateAccount(long accountId, string name)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", accountId, DbType.Int64);
            parameters.Add("@name", name, DbType.Int64);

            await _accountDb.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_finance].[CreateAccount]",
                param: parameters,
                transaction: _financeDb.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        private class AccountNameItem
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}
