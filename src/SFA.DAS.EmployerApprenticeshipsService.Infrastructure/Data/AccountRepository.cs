using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
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
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private readonly EmployerAccountDbContext _employerAccountDbContext;
        private readonly IHashingService _accountLegalEntityHashingService;

        public AccountRepository(
            EmployerApprenticeshipsServiceConfiguration configuration, 
            ILog logger, 
            EmployerAccountDbContext employerAccountDbContext,
            IHashingService accountLegalEntityHashingService)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _employerAccountDbContext = employerAccountDbContext;
            _accountLegalEntityHashingService = accountLegalEntityHashingService;
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
            return await WithTransaction(async (c, t) =>
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
                parameters.Add("@accountLegalentityId", null, DbType.Int64, ParameterDirection.Output, 8);
                parameters.Add("@employerAgreementId", null, DbType.Int64, ParameterDirection.Output, 8);
                parameters.Add("@accessToken", accessToken, DbType.String);
                parameters.Add("@refreshToken", refreshToken, DbType.String);
                parameters.Add("@addedDate", DateTime.UtcNow, DbType.DateTime);
                parameters.Add("@employerRefName", employerRefName, DbType.String);
                parameters.Add("@status", companyStatus);
                parameters.Add("@source", source);
                parameters.Add("@publicSectorDataSource", publicSectorDataSource);
                parameters.Add("@sector", sector, DbType.String);
                
                await c.ExecuteAsync(
                    sql: "[employer_account].[CreateAccount]",
                    param: parameters,
                    transaction: t,
                    commandType: CommandType.StoredProcedure);

                var accountLegalEntityId = parameters.Get<long>("@accountLegalentityId");

                await UpdateAccountLegalEntityPublicHashedIdInternal(c, t, accountLegalEntityId);

                return new CreateAccountResult
                {
                    AccountId = parameters.Get<long>("@accountId"),
                    LegalEntityId = parameters.Get<long>("@legalentityId"),
                    EmployerAgreementId = parameters.Get<long>("@employerAgreementId")
                };
            });
        }

        public async Task<EmployerAgreementView> CreateLegalEntityWithAgreement(CreateLegalEntityWithAgreementParams createParams)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", createParams.AccountId, DbType.Int64);
                parameters.Add("@companyNumber", createParams.Code, DbType.String);
                parameters.Add("@companyName", createParams.Name, DbType.String);
                parameters.Add("@CompanyAddress", createParams.Address, DbType.String);
                parameters.Add("@CompanyDateOfIncorporation", createParams.DateOfIncorporation, DbType.DateTime);
                parameters.Add("@legalEntityId", null, DbType.Int64, ParameterDirection.Output);
                parameters.Add("@employerAgreementId", null, DbType.Int64, ParameterDirection.Output);
                parameters.Add("@status", createParams.Status, DbType.String);
                parameters.Add("@source", createParams.Source, DbType.Int16);
                parameters.Add("@publicSectorDataSource", createParams.PublicSectorDataSource, DbType.Int16);
                parameters.Add("@sector", createParams.Sector, DbType.String);
                parameters.Add("@accountLegalentityId", null, DbType.Int64, ParameterDirection.Output);
                parameters.Add("@accountLegalEntityCreated", null, DbType.Boolean, ParameterDirection.Output);

                var trans = c.BeginTransaction();

                var result = await c.ExecuteAsync(
                    sql: "[employer_account].[CreateLegalEntityWithAgreement]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);

                var legalEntityId = parameters.Get<long>("@legalEntityId");
                var agreementId = parameters.Get<long>("@employerAgreementId");
                var accountLegalEntityId = parameters.Get<long>("@accountLegalentityId");
                var accountLegalEntityCreated = parameters.Get<bool>("@accountLegalEntityCreated");

                if (accountLegalEntityCreated)
                {
                    await UpdateAccountLegalEntityPublicHashedIdInternal(c, trans, accountLegalEntityId);
                }

                trans.Commit();

                return new EmployerAgreementView
                {
                    Id = agreementId,
                    AccountId = createParams.AccountId,
                    AccountLegalentityId = accountLegalEntityId,
                    LegalEntityId = legalEntityId,
                    LegalEntityName = createParams.Name,
                    LegalEntityCode = createParams.Code,
                    LegalEntityAddress = createParams.Address,
                    LegalEntityInceptionDate = createParams.DateOfIncorporation,
                    Sector = createParams.Sector,
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
            var result = await WithConnection(c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@UserRef", Guid.Parse(userRef), DbType.Guid);

                return c.QueryAsync<UserNotificationSetting>(
                    sql: "[employer_account].[GetUserAccountSettings]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public async Task RemovePayeFromAccount(long accountId, string payeRef)
        {
            await WithConnection(c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountId", accountId, DbType.Int64);
                parameters.Add("@PayeRef", payeRef, DbType.String);
                parameters.Add("@RemovedDate", DateTime.UtcNow, DbType.DateTime);

                return c.ExecuteAsync(
                   sql: "[employer_account].[UpdateAccountHistory]",
                   param: parameters,
                   commandType: CommandType.StoredProcedure);
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

            await WithTransaction(async (c, t) =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@UserRef", Guid.Parse(userRef), DbType.Guid);
                parameters.Add("@NotificationSettings", settingsDataTable.AsTableValuedParameter("employer_account.UserNotificationSettingsTable"));

                await c.ExecuteAsync(
                    sql: "[employer_account].[UpdateUserAccountSettings]",
                    param: parameters,
                    transaction: t,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<string> GetAccountName(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<string>(
                    sql: "SELECT Name FROM [employer_account].[Account] WHERE Id = @accountId",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task<Dictionary<long, string>> GetAccountNames(IEnumerable<long> accountIds)
        {
            var result = await WithConnection(async c =>
            {
                return await c.QueryAsync<AccountNameItem>(
                     "SELECT Id, Name FROM [employer_account].[Account] WHERE Id IN @accountIds"
                    , new { accountIds = accountIds });
            });

            return result.ToDictionary(data => data.Id, data => data.Name);
        }

        public Task UpdateLegalEntityDetailsForAccount(long accountLegalEntityId, string address, string name)
        {
            return WithTransaction((c, t) =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountLegalEntityId", accountLegalEntityId, DbType.Int64);
                parameters.Add("@Name", name, DbType.String);
                parameters.Add("@Address", address, DbType.String);

                return c.ExecuteAsync(
                    sql: "[employer_account].[UpdateAccountLegalEntity_SetNameAndAddress]",
                    param: parameters,
                    transaction: t,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public Task UpdateAccountLegalEntityPublicHashedId(long accountLegalEntityId)
        {
            return WithTransaction( (c,t) => UpdateAccountLegalEntityPublicHashedIdInternal(c, t, accountLegalEntityId));
        }

        private Task UpdateAccountLegalEntityPublicHashedIdInternal(IDbConnection connection, IDbTransaction transaction, long accountLegalEntityId)
        {
            var parameters = new DynamicParameters();

            var publicHash = _accountLegalEntityHashingService.HashValue(accountLegalEntityId);

            parameters.Add("@AccountLegalEntityId", accountLegalEntityId, DbType.Int64);
            parameters.Add("@PublicHashedId", publicHash, DbType.String);

            return connection.ExecuteAsync(
                sql: "[employer_account].[UpdateAccountLegalEntity_SetPublicHashedId]",
                param: parameters,
                transaction: transaction,
                commandType: CommandType.StoredProcedure);
        }

        public Task UpdateLegalEntityDetailsForAccount(long accountId, long legalEntityId, string address, string name)
        {
            //[employer_account].[UpdateAccountLegalEntity_SetNameAndAddress]
            return WithTransaction(async (c, t) =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountId", accountId, DbType.Int64);
                parameters.Add("@LegalEntityId", legalEntityId, DbType.Int64);
                parameters.Add("@Name", name, DbType.String);
                parameters.Add("@Address", address, DbType.String);

                await c.ExecuteAsync(
                    sql: "[employer_account].[UpdateAccountLegalEntity_SetNameAndAddress]",
                    param: parameters,
                    transaction: t,
                    commandType: CommandType.StoredProcedure);
            });
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class AccountNameItem
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}