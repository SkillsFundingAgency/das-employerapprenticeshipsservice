﻿using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Models;
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

        public async Task<AccountDetail> GetAccountDetailByHashedId(string hashedAccountId)
        {
            var sw = new Stopwatch();

            sw.Start();
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


            var templateIds = await _db.Value.Agreements
                .Where(x => accountDetail.LegalEntities.Contains(x.AccountLegalEntity.LegalEntityId))
                .Select(x => x.TemplateId)
                .ToListAsync()
                .ConfigureAwait(false)
            ;

            accountDetail.AccountAgreementTypes = await _db.Value.AgreementTemplates
                .Where(x => templateIds.Contains(x.Id))
                .Select(x => x.AgreementType)
                .ToListAsync()
                .ConfigureAwait(false)
            ;

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
    }
}