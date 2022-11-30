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
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public AccountRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<string> GetAccountName(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<string>(
                sql: "SELECT Name FROM [employer_financial].[Account] WHERE Id = @accountId",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public async Task<Dictionary<long, string>> GetAccountNames(IEnumerable<long> accountIds)
        {
            var result = await _db.Value.Database.Connection.QueryAsync<AccountNameItem>(
                sql: "SELECT Id, Name FROM [employer_financial].[Account] WHERE Id IN @accountIds",
                param: new { accountIds = accountIds },
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction);

            return result.ToDictionary(d => d.Id, d => d.Name);
        }

        public async Task CreateAccount(long accountId, string name)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", accountId, DbType.Int64);
            parameters.Add("@name", name, DbType.String);

            await _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[CreateAccount]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task RenameAccount(long accountId, string name)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", accountId, DbType.Int64);
            parameters.Add("@name", name, DbType.String);

            await _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[RenameAccount]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        private class AccountNameItem
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}
