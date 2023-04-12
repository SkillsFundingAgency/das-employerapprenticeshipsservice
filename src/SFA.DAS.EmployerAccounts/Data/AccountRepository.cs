using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Data;

public class AccountRepository : IAccountRepository
{
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly IEncodingService _encodingService;

   public AccountRepository(Lazy<EmployerAccountsDbContext> db, IEncodingService encodingService)
    {
        _db = db;
        _encodingService = encodingService;
    }

    public async Task<CreateUserAccountResult> CreateUserAccount(long userId, string employerName)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@userId", userId, DbType.Int64);
        parameters.Add("@employerName", employerName, DbType.String);
        parameters.Add("@apprenticeshipEmployerType", ApprenticeshipEmployerType.Unknown, DbType.Int16);
        parameters.Add("@accountId", null, DbType.Int64, ParameterDirection.Output, 8);
        parameters.Add("@addedDate", DateTime.UtcNow, DbType.DateTime);

        await _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[CreateUserAccount]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);

        return new CreateUserAccountResult
        {
            AccountId = parameters.Get<long>("@accountId")
        };
    }

    public async Task<CreateAccountResult> CreateAccount(CreateAccountParams createParams)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@userId", createParams.UserId, DbType.Int64);
        parameters.Add("@employerNumber", createParams.EmployerNumber, DbType.String);
        parameters.Add("@employerName", createParams.EmployerName, DbType.String);
        parameters.Add("@employerRegisteredAddress", createParams.EmployerRegisteredAddress, DbType.String);
        parameters.Add("@employerDateOfIncorporation", createParams.EmployerDateOfIncorporation, DbType.DateTime);
        parameters.Add("@employerRef", createParams.EmployerRef, DbType.String);
        parameters.Add("@accountId", null, DbType.Int64, ParameterDirection.Output, 8);
        parameters.Add("@legalentityId", null, DbType.Int64, ParameterDirection.Output, 8);
        parameters.Add("@accountLegalentityId", null, DbType.Int64, ParameterDirection.Output, 8);
        parameters.Add("@employerAgreementId", null, DbType.Int64, ParameterDirection.Output, 8);
        parameters.Add("@accessToken", createParams.AccessToken, DbType.String);
        parameters.Add("@refreshToken", createParams.RefreshToken, DbType.String);
        parameters.Add("@addedDate", DateTime.UtcNow, DbType.DateTime);
        parameters.Add("@employerRefName", createParams.EmployerRefName, DbType.String);
        parameters.Add("@status", createParams.CompanyStatus);
        parameters.Add("@source", createParams.Source);
        parameters.Add("@publicSectorDataSource", createParams.PublicSectorDataSource);
        parameters.Add("@sector", createParams.Sector, DbType.String);
        parameters.Add("@aorn", createParams.Aorn, DbType.String);
        parameters.Add("@agreementType", createParams.AgreementType, DbType.Int16);
        parameters.Add("@apprenticeshipEmployerType", createParams.ApprenticeshipEmployerType, DbType.Int16);
        parameters.Add("@agreementVersion", null, DbType.Int32, ParameterDirection.Output);

        await _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[CreateAccount]",
            param: parameters,
            commandType: CommandType.StoredProcedure,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction());

        var accountLegalEntityId = parameters.Get<long>("@accountLegalentityId");

        await UpdateAccountLegalEntityPublicHashedIdInternal(_db.Value.Database.GetDbConnection(), accountLegalEntityId);

        return new CreateAccountResult
        {
            AccountId = parameters.Get<long>("@accountId"),
            LegalEntityId = parameters.Get<long>("@legalentityId"),
            EmployerAgreementId = parameters.Get<long>("@employerAgreementId"),
            AgreementVersion = parameters.Get<int>("@agreementVersion"),
            AccountLegalEntityId = accountLegalEntityId
        };
    }

    public async Task<EmployerAgreementView> CreateLegalEntityWithAgreement(CreateLegalEntityWithAgreementParams createParams)
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
        parameters.Add("@agreementType", createParams.AgreementType, DbType.Int16);
        parameters.Add("@accountLegalentityId", null, DbType.Int64, ParameterDirection.Output);
        parameters.Add("@accountLegalEntityCreated", null, DbType.Boolean, ParameterDirection.Output);
        parameters.Add("@agreementVersion", null, DbType.Int32, ParameterDirection.Output);

        await _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[CreateLegalEntityWithAgreement]",
            param: parameters,
            commandType: CommandType.StoredProcedure,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction());

        var legalEntityId = parameters.Get<long>("@legalEntityId");
        var agreementId = parameters.Get<long>("@employerAgreementId");
        var accountLegalEntityId = parameters.Get<long>("@accountLegalentityId");
        var accountLegalEntityCreated = parameters.Get<bool>("@accountLegalEntityCreated");

        if (accountLegalEntityCreated)
        {
            await UpdateAccountLegalEntityPublicHashedIdInternal(_db.Value.Database.GetDbConnection(), accountLegalEntityId);
        }

        return new EmployerAgreementView
        {
            Id = agreementId,
            AccountId = createParams.AccountId,
            AccountLegalEntityId = accountLegalEntityId,
            LegalEntityId = legalEntityId,
            LegalEntityName = createParams.Name,
            LegalEntityCode = createParams.Code,
            LegalEntitySource = createParams.Source,
            LegalEntityAddress = createParams.Address,
            LegalEntityInceptionDate = createParams.DateOfIncorporation,
            Sector = createParams.Sector,
            Status = EmployerAgreementStatus.Pending,
            VersionNumber = parameters.Get<int>("@agreementVersion")
        };
    }

    public async Task<List<PayeView>> GetPayeSchemesByAccountId(long accountId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@accountId", accountId, DbType.Int64);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<PayeView>(
            sql: "[employer_account].[GetPayeSchemes_ByAccountId]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    public async Task<List<UserNotificationSetting>> GetUserAccountSettings(string userRef)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@UserRef", Guid.Parse(userRef), DbType.Guid);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<UserNotificationSetting>(
            sql: "[employer_account].[GetUserAccountSettings]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    public Task UpdateAccountHashedIds(long accountId, string hashedId, string publicHashedId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@accountId", accountId, DbType.Int64);
        parameters.Add("@hashedId", hashedId, DbType.String);
        parameters.Add("@publicHashedId", publicHashedId, DbType.String);

        return _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[UpdateAccountHashedIds]",
            param: parameters,
            commandType: CommandType.StoredProcedure,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction());
    }

    public Task UpdateUserAccountSettings(string userRef, List<UserNotificationSetting> settings)
    {
        var settingsDataTable = new DataTable();

        settingsDataTable.Columns.Add("AccountId", typeof(long));
        settingsDataTable.Columns.Add("ReceiveNotifications", typeof(bool));

        foreach (var setting in settings)
        {
            settingsDataTable.Rows.Add(setting.AccountId, setting.ReceiveNotifications);
        }

        var parameters = new DynamicParameters();

        parameters.Add("@UserRef", Guid.Parse(userRef), DbType.Guid);
        parameters.Add("@NotificationSettings", settingsDataTable.AsTableValuedParameter("employer_account.UserNotificationSettingsTable"));

        return _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[UpdateUserAccountSettings]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);
    }

    public Task UpdateLegalEntityDetailsForAccount(long accountLegalEntityId, string name, string address)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@AccountLegalEntityId", accountLegalEntityId, DbType.Int64);
        parameters.Add("@Name", name, DbType.String);
        parameters.Add("@Address", address, DbType.String);

        return _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[UpdateAccountLegalEntity_SetNameAndAddress]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);
    }

    private Task UpdateAccountLegalEntityPublicHashedIdInternal(IDbConnection connection, long accountLegalEntityId)
    {
        var parameters = new DynamicParameters();

        var publicHash = _encodingService.Encode(accountLegalEntityId, EncodingType.PublicAccountLegalEntityId);

        parameters.Add("@AccountLegalEntityId", accountLegalEntityId, DbType.Int64);
        parameters.Add("@PublicHashedId", publicHash, DbType.String);

        return connection.ExecuteAsync(
            sql: "[employer_account].[UpdateAccountLegalEntity_SetPublicHashedId]",
            param: parameters,
            commandType: CommandType.StoredProcedure,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction());
    }
}