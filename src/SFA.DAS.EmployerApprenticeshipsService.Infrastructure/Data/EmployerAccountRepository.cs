using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Account;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class EmployerAccountRepository : BaseRepository, IEmployerAccountRepository
    {
        
        public EmployerAccountRepository(EmployerApprenticeshipsServiceConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<Account> GetAccountById(long id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int64);

                return await c.QueryAsync<Account>(
                    sql: "select a.* from [employer_account].[Account] a where a.Id = @id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            return result.SingleOrDefault();
        }

        public async Task<Account> GetAccountByHashedId(string hashedAccountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HashedAccountId", hashedAccountId, DbType.String);

                return await c.QueryAsync<Account>(
                    sql: "select a.* from [employer_account].[Account] a where a.HashedId = @HashedAccountId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            return result.SingleOrDefault();
        }

        public async Task<Accounts<Account>> GetAccounts(string toDate, int pageNumber, int pageSize)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@toDate", toDate);
            var offset = pageSize * (pageNumber - 1);

            var countResult = await GetNumberOfAccounts();

            var result = await WithConnection(async c => await c.QueryAsync<Account>(
                sql:    $"select a.* from [employer_account].[Account] a ORDER BY a.Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY;", 
                commandType: CommandType.Text));

            return new Accounts<Account> {AccountsCount = countResult.First(), AccountList = result.ToList()};
        }

        public async Task<Accounts<AccountInformation>> GetAccountsByDateRange(DateTime fromDate, DateTime toDate, int pageNumber, int pageSize)
        {
            var countResult = await GetNumberOfAccounts();
            var offset = pageSize * (pageNumber - 1);

            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@fromDate", fromDate, DbType.DateTime);
                parameters.Add("@toDate", toDate, DbType.DateTime);
                parameters.Add("@offset",offset,DbType.Int32);
                parameters.Add("@pageSize", pageSize, DbType.Int32);

                return await c.QueryAsync<AccountInformation>(
                    sql: "[employer_account].[GetAccountInformation_ByDateRange]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            }
                );

            return new Accounts<AccountInformation> {AccountsCount = countResult.First(), AccountList = result.ToList()};
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            var result = await WithConnection(async c => 
                await c.QueryAsync<Account>("select * from [employer_account].[Account]", commandType: CommandType.Text));

            return result.AsList();
        }

        public async Task<List<AccountHistoryEntry>> GetAccountHistory(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<AccountHistoryEntry>(
                    sql: "[employer_account].[GetAccountHistory]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public async Task RenameAccount(long accountId, string name)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@accountName", name, DbType.String);

                return await c.ExecuteAsync(
                    sql: "[employer_account].[UpdateAccount_SetAccountName]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
		}

        private async Task<IEnumerable<int>> GetNumberOfAccounts()
        {
            var countResult =
                await WithConnection(async c => await c.QueryAsync<int>(sql: $"select count(*) from [employer_account].[Account] a;"));
            return countResult;
        }
    }
}

