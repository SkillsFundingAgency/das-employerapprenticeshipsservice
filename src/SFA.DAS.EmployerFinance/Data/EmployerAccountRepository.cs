using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public class EmployerAccountRepository : BaseRepository, IEmployerAccountRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public EmployerAccountRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            var result = await _db.Value.Database.Connection.QueryAsync<Account>(
                sql: "select * from [employer_account].[Account]",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.AsList();
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
    }
}
