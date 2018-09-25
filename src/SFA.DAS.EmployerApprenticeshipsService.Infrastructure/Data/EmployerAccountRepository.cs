using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class EmployerAccountRepository : BaseRepository, IEmployerAccountRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly ILog _logger;

        public EmployerAccountRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
            _logger = logger;
        }

        public Task<Account> GetAccountById(long id)
        {
            return _db.Value.Accounts.SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Domain.Models.Account.Account> GetAccountByHashedId(string hashedAccountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@HashedAccountId", hashedAccountId, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<Domain.Models.Account.Account>(
                sql: "select a.* from [employer_account].[Account] a where a.HashedId = @HashedAccountId;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public async Task<Accounts<Domain.Models.Account.Account>> GetAccounts(string toDate, int pageNumber, int pageSize)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@toDate", toDate);

            var offset = pageSize * (pageNumber - 1);

            var countResult = await _db.Value.Database.Connection.QueryAsync<int>(
                sql: $"select count(*) from [employer_account].[Account] a;",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction);

            var result = await _db.Value.Database.Connection.QueryAsync<Domain.Models.Account.Account>(
                sql: $"select a.* from [employer_account].[Account] a ORDER BY a.Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY;",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction);

            return new Accounts<Domain.Models.Account.Account>
            {
                AccountsCount = countResult.First(),
                AccountList = result.ToList()
            };
        }

        public async Task<AccountDetail> GetAccountDetailByHashedId(string hashedAccountId)
        {
            var sw = new Stopwatch();
            AccountDetail accountDetail = null;

            sw.Start();
            accountDetail = await _db.Value.Accounts
                .Where(ac => ac.HashedId == hashedAccountId)
                .Select(ac => new AccountDetail
                {
                    AccountId = ac.Id,
                    HashedId = ac.HashedId,
                    PublicHashedId = ac.PublicHashedId,
                    Name = ac.Name,
                    CreatedDate = ac.CreatedDate
                }).FirstOrDefaultAsync();

            if (accountDetail == null)
            {
                _logger.Warn($"An attempt to fetch an account using an unknown account - {hashedAccountId}");
                return null;
            }

            accountDetail.OwnerEmail = await _db.Value.Memberships
                .Where(m => m.AccountId == accountDetail.AccountId && m.Role == Role.Owner)
                .OrderBy(m => m.CreatedDate)
                .Select(m => m.User.Email)
                .FirstOrDefaultAsync();

            accountDetail.PayeSchemes = await _db.Value.AccountHistory
                .Where(ach => ach.AccountId == accountDetail.AccountId)
                .Select(ach => ach.PayeRef)
                .ToListAsync();

            accountDetail.LegalEntities = await _db.Value.AccountLegalEntities
                .Where(ale => ale.AccountId == accountDetail.AccountId
                                && ale.Deleted == null
                                && ale.Agreements.Any(ea =>
                                    ea.StatusId == EmployerAgreementStatus.Pending ||
                                    ea.StatusId == EmployerAgreementStatus.Signed))
                .Select(ale => ale.LegalEntityId)
                .ToListAsync();

            sw.Stop();
            _logger.Debug($"Fetched account with {accountDetail.LegalEntities.Count} legal entities and {accountDetail.PayeSchemes.Count} PAYE schemes in {sw.ElapsedMilliseconds} msecs");

            return accountDetail;
        }

        public async Task<List<Domain.Models.Account.Account>> GetAllAccounts()
        {
            var result = await _db.Value.Database.Connection.QueryAsync<Domain.Models.Account.Account>(
                sql: "select * from [employer_account].[Account]",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.AsList();
        }

        public async Task<List<AccountHistoryEntry>> GetAccountHistory(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<AccountHistoryEntry>(
                sql: "[employer_account].[GetAccountHistory]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
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
    }
}