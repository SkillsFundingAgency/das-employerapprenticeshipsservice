﻿using Dapper;
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
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

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

        public async Task<Accounts<Account>> GetAccounts(string toDate, int pageNumber, int pageSize)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@toDate", toDate);

            var offset = pageSize * (pageNumber - 1);

            var countResult = await _db.Value.Database.Connection.QueryAsync<int>(
                sql: $"select count(*) from [employer_account].[Account] a;",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction);

            var result = _db.Value.Accounts
                .Include(x => x.AccountLegalEntities.Select(y => y.Agreements))
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(pageSize);

            return new Accounts<Account>
            {
                AccountsCount = countResult.First(),
                AccountList = result.ToList()
            };
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

        public async Task<AccountDetail> GetAccountDetailByHashedId(string hashedAccountId)
        {        
            var accountDetail = await _db.Value.Accounts
                .Where(ac => ac.HashedId == hashedAccountId)
                .Select(ac => new AccountDetail
                {
                    AccountId = ac.Id,
                    HashedId = ac.HashedId,
                    PublicHashedId = ac.PublicHashedId,
                    Name = ac.Name,
                    CreatedDate = ac.CreatedDate,
                    ApprenticeshipEmployerType = (ApprenticeshipEmployerType) ac.ApprenticeshipEmployerType
                }).FirstOrDefaultAsync();

            if (accountDetail == null)
            {             
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

            var templateIds = await _db.Value.Agreements
                .Where(x => accountDetail.LegalEntities.Contains(x.AccountLegalEntity.LegalEntityId))
                .Select(x => x.TemplateId)
                .ToListAsync()
                .ConfigureAwait(false);

            accountDetail.AccountAgreementTypes = await _db.Value.AgreementTemplates
                .Where(x => templateIds.Contains(x.Id))
                .Select(x => x.AgreementType)
                .ToListAsync()
                .ConfigureAwait(false);
              
            return accountDetail;
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
