using Dapper;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class EmployerAccountRepository : BaseRepository, IEmployerAccountRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public EmployerAccountRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public Task<Account> GetAccountById(long id)
        {
            return _db.Value.Accounts.SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            var result = await _db.Value.Database.Connection.QueryAsync<Account>(
                sql: "select * from [employer_account].[Account]",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.AsList();
        }

        public async Task<Accounts<Account>> GetAccounts(string toDate, int pageNumber, int pageSize)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@toDate", toDate);

            var offset = pageSize * (pageNumber - 1);

            var countResult = await _db.Value.Database.Connection.QueryAsync<int>(
                sql: $"select count(*) from [employer_account].[Account] a;",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction);

            var result = await _db.Value.Database.Connection.QueryAsync<Account>(
                sql: $"select a.* from [employer_account].[Account] a ORDER BY a.Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY;",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction);

            return new Accounts<Account>
            {
                AccountsCount = countResult.First(),
                AccountList = result.ToList()
            };
        }

        public async Task<Account> GetAccountByHashedId(string hashedAccountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@HashedAccountId", hashedAccountId, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<Account>(
                sql: "select a.* from [employer_account].[Account] a where a.HashedId = @HashedAccountId;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }


        public async Task<AccountStats> GetAccountStats(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<AccountStats>(
                sql: "[employer_account].[GetAccountStats]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }

        public Task RenameAccount(long accountId, string name)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@accountName", name, DbType.String);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[UpdateAccount_SetAccountName]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public Task SetAccountAsLevy(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@ApprenticeshipEmployerType", ApprenticeshipEmployerType.Levy, DbType.Byte);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[UpdateAccount_SetAccountApprenticeshipEmployerType]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}
