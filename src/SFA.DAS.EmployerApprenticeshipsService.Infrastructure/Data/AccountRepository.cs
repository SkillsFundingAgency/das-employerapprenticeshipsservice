﻿using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.Domain.Models.Settings;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task AddPayeToAccount(Paye payeScheme)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@accountId", payeScheme.AccountId, DbType.Int64);
                parameters.Add("@employerRef", payeScheme.EmpRef, DbType.String);
                parameters.Add("@accessToken", payeScheme.AccessToken, DbType.String);
                parameters.Add("@refreshToken", payeScheme.RefreshToken, DbType.String);
                parameters.Add("@addedDate", DateTime.UtcNow, DbType.DateTime);
                parameters.Add("@employerRefName", payeScheme.RefName, DbType.String);

                var trans = c.BeginTransaction();

                var result = await c.ExecuteAsync(
                    sql: "[employer_account].[AddPayeToAccount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);

                trans.Commit();

                return result;
            });
        }

        public async Task<CreateAccountResult> CreateAccount(long userId, string employerNumber, string employerName, string employerRegisteredAddress, DateTime? employerDateOfIncorporation, string employerRef, string accessToken, string refreshToken, string companyStatus, string employerRefName, short source, short? publicSectorDataSource, string sector)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@userId", userId, DbType.Int64);
                parameters.Add("@employerNumber", employerNumber, DbType.String);
                parameters.Add("@employerName", employerName, DbType.String);
                parameters.Add("@employerRegisteredAddress", employerRegisteredAddress, DbType.String);
                parameters.Add("@employerDateOfIncorporation", employerDateOfIncorporation, DbType.DateTime);
                parameters.Add("@employerRef", employerRef, DbType.String);
                parameters.Add("@accountId", null, DbType.Int64, ParameterDirection.Output, 8);
                parameters.Add("@legalentityId", null, DbType.Int64, ParameterDirection.Output, 8);
                parameters.Add("@employerAgreementId", null, DbType.Int64, ParameterDirection.Output, 8);
                parameters.Add("@accessToken", accessToken, DbType.String);
                parameters.Add("@refreshToken", refreshToken, DbType.String);
                parameters.Add("@addedDate", DateTime.UtcNow, DbType.DateTime);
                parameters.Add("@employerRefName", employerRefName, DbType.String);
                parameters.Add("@status", companyStatus);
                parameters.Add("@source", source);
                parameters.Add("@publicSectorDataSource", publicSectorDataSource);
                parameters.Add("@sector", sector, DbType.String);

                var trans = c.BeginTransaction();

                await c.ExecuteAsync(
                    sql: "[employer_account].[CreateAccount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);

                trans.Commit();

                return new CreateAccountResult
                {
                    AccountId = parameters.Get<long>("@accountId"),
                    LegalEntityId = parameters.Get<long>("@legalentityId"),
                    EmployerAgreementId = parameters.Get<long>("@employerAgreementId")
                };
            });
        }

        public async Task<EmployerAgreementView> CreateLegalEntity(long accountId, LegalEntity legalEntity)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@companyNumber", legalEntity.Code, DbType.String);
                parameters.Add("@companyName", legalEntity.Name, DbType.String);
                parameters.Add("@CompanyAddress", legalEntity.RegisteredAddress, DbType.String);
                parameters.Add("@CompanyDateOfIncorporation", legalEntity.DateOfIncorporation, DbType.DateTime);
                parameters.Add("@legalEntityId", null, DbType.Int64, ParameterDirection.Output);
                parameters.Add("@employerAgreementId", null, DbType.Int64, ParameterDirection.Output);
                parameters.Add("@status", legalEntity.CompanyStatus, DbType.String);
                parameters.Add("@source", legalEntity.Source, DbType.Int16);
                parameters.Add("@publicSectorDataSource", legalEntity.PublicSectorDataSource, DbType.Int16);
                parameters.Add("@sector", legalEntity.Sector, DbType.String);

                var trans = c.BeginTransaction();

                var result = await c.ExecuteAsync(
                    sql: "[employer_account].[CreateLegalEntityWithAgreement]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);

                trans.Commit();

                var legalEntityId = parameters.Get<long>("@legalEntityId");
                var agreementId = parameters.Get<long>("@employerAgreementId");

                return new EmployerAgreementView
                {
                    Id = agreementId,
                    AccountId = accountId,
                    LegalEntityId = legalEntityId,
                    LegalEntityName = legalEntity.Name,
                    LegalEntityCode = legalEntity.Code,
                    LegalEntityAddress = legalEntity.RegisteredAddress,
                    LegalEntityInceptionDate = legalEntity.DateOfIncorporation,
                    Sector = legalEntity.Sector,
                    Status = EmployerAgreementStatus.Pending,
                };
            });
        }

        public async Task<AccountStats> GetAccountStats(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<AccountStats>(
                    sql: "[employer_account].[GetAccountStats]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }

        public async Task<List<EmployerAgreementView>> GetEmployerAgreementsLinkedToAccount(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<EmployerAgreementView>(
                    sql: "[employer_account].[GetEmployerAgreementsLinkedToAccount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public async Task<List<PayeView>> GetPayeSchemesByAccountId(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<PayeView>(
                    sql: "[employer_account].[GetPayeSchemes_ByAccountId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public async Task<List<UserNotificationSetting>> GetUserAccountSettings(string userRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@UserRef", Guid.Parse(userRef), DbType.Guid);

                return await c.QueryAsync<UserNotificationSetting>(
                    sql: "[employer_account].[GetUserAccountSettings]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public async Task RemovePayeFromAccount(long accountId, string payeRef)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountId", accountId, DbType.Int64);
                parameters.Add("@PayeRef", payeRef, DbType.String);
                parameters.Add("@RemovedDate", DateTime.UtcNow, DbType.DateTime);

                var result = await c.ExecuteAsync(
                   sql: "[employer_account].[UpdateAccountHistory]",
                   param: parameters,
                   commandType: CommandType.StoredProcedure);

                return result;
            });
        }

        public async Task UpdateAccountHashedIds(long accountId, string hashedId, string publicHashedId)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@hashedId", hashedId, DbType.String);
                parameters.Add("@publicHashedId", publicHashedId, DbType.String);

                var result = await c.ExecuteAsync(
                    sql: "[employer_account].[UpdateAccountHashedIds]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            });
        }

        public async Task UpdateUserAccountSettings(string userRef, List<UserNotificationSetting> settings)
        {
            var settingsDataTable = new DataTable();

            settingsDataTable.Columns.Add("AccountId", typeof(long));
            settingsDataTable.Columns.Add("ReceiveNotifications", typeof(bool));

            foreach (var setting in settings)
            {
                settingsDataTable.Rows.Add(setting.AccountId, setting.ReceiveNotifications);
            }

            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@UserRef", Guid.Parse(userRef), DbType.Guid);
                parameters.Add("@NotificationSettings", settingsDataTable.AsTableValuedParameter("employer_account.UserNotificationSettingsTable"));

                return await c.ExecuteAsync(
                    sql: "[employer_account].[UpdateUserAccountSettings]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<Dictionary<long, string>> GetAccountNames(IEnumerable<long> accountIds)
        {
            var result = await WithConnection(async c =>
            {
                // var parameters = new DynamicParameters();

                //parameters.Add("@accountIds", accountIds, DbType.);

                return await c.QueryAsync<AccountNameItem>(
                     "SELECT Id, Name FROM [employer_account].[Account] WHERE Id IN @accountIds"
                    , new { accountIds = accountIds });
            });

            return result.ToDictionary(data => data.Id, data => data.Name);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class AccountNameItem
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}