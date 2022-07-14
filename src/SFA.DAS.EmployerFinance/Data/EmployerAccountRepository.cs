using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class EmployerAccountRepository : BaseRepository, IEmployerAccountRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public EmployerAccountRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
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
                sql: "select * from [employer_financial].[Account]",
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.AsList();
        }
    }
}
