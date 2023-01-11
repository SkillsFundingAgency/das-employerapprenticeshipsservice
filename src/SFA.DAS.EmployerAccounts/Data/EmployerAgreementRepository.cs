using System.Data;
using System.Data.Entity;
using Dapper;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.Organisation;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.Data;

public class EmployerAgreementRepository : BaseRepository, IEmployerAgreementRepository
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public EmployerAgreementRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db) 
        : base(configuration.DatabaseConnectionString, logger)
    {
        _db = db;
    }

    public async Task<List<AccountSpecificLegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId,
        bool signedOnly)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@accountId", accountId, DbType.Int64);

        var sql = @"
                    SELECT	le.Id, le.Code, le.DateOfIncorporation, le.PublicSectorDataSource, le.Sector, le.Source, le.Status,
	                        ale.Name, ale.Address, ale.SignedAgreementVersion, ale.SignedAgreementId, ale.PendingAgreementVersion, ale.PendingAgreementId, ale.PublicHashedId as AccountLegalEntityPublicHashedId
                    FROM	[employer_account].[AccountLegalEntity] AS ale
	                        JOIN [employer_account].[LegalEntity] AS le
		                        ON le.Id = ale.LegalEntityId
                    WHERE	ale.AccountId = @accountId and ale.Deleted IS NULL";

        if (signedOnly)
        {
            sql += " AND ale.SignedAgreementId IS NOT NULL";
        }

        var result = await _db.Value.Database.Connection.QueryAsync<AccountSpecificLegalEntity>(
            sql: sql,
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
            commandType: CommandType.Text);

        return result.ToList();
    }

    public async Task<EmployerAgreementView> GetEmployerAgreement(long agreementId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@agreementId", agreementId, DbType.Int64);

        var result = await _db.Value.Database.Connection.QueryAsync<EmployerAgreementView>(
            sql: "[employer_account].[GetEmployerAgreement]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
            commandType: CommandType.StoredProcedure);

        return result.SingleOrDefault();
    }

    public Task SignAgreement(SignEmployerAgreement agreement)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@agreementId", agreement.AgreementId, DbType.Int64);
        parameters.Add("@signedById", agreement.SignedById, DbType.Int64);
        parameters.Add("@signedByName", agreement.SignedByName, DbType.String);
        parameters.Add("@signedDate", agreement.SignedDate, DbType.DateTime);
            
        return _db.Value.Database.Connection.ExecuteAsync(
            sql: "[employer_account].[SignEmployerAgreement]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
            commandType: CommandType.StoredProcedure);
    }

    public Task RemoveLegalEntityFromAccount(long agreementId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@employerAgreementId", agreementId, DbType.Int64);

        return _db.Value.Database.Connection.ExecuteAsync(
            sql: "[employer_account].[RemoveLegalEntityFromAccount]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
            commandType: CommandType.StoredProcedure);
    }
        
    public async Task<AccountLegalEntityModel> GetAccountLegalEntity(long accountLegalEntityId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@id", accountLegalEntityId, DbType.Int64);

        var result = await _db.Value.Database.Connection.QueryAsync<AccountLegalEntityModel>(
            sql: @"
                SELECT  ALE.Id as AccountLegalEntityId, 
                        LE.Id AS LegalEntityId, 
                        ALE.Name, 
                        ALE.PublicHashedId AS AccountLegalEntityPublicHashedId, 
                        ALE.Name, 
                        ALE.Address, 
                        LE.Source AS OrganisationType, 
                        LE.Code AS Identifier 
                FROM    [employer_account].[AccountLegalEntity] AS ALE 
                        JOIN [employer_account].[LegalEntity] AS LE
                            ON LE.Id = ALE.LegalEntityId
                    WHERE ALE.Id = @Id;",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
            commandType: CommandType.Text);

        return result.SingleOrDefault();
    }

    public async Task<IEnumerable<EmployerAgreement>> GetAccountAgreements(long accountId)
    {
        var legalEntities = await _db.Value.AccountLegalEntities
            .Where(ale => ale.AccountId == accountId
                          && ale.Deleted == null
                          && ale.Agreements.Any(ea =>
                              ea.StatusId == EmployerAgreementStatus.Pending ||
                              ea.StatusId == EmployerAgreementStatus.Signed))
            .ToListAsync()
            .ConfigureAwait(false);

        var agreements = legalEntities.SelectMany(x => x.Agreements).ToList();
        return agreements;
    }

    public async Task<IEnumerable<EmployerAgreement>> GetAccountLegalEntityAgreements(long accountLegalEntityId)
    {
        return await _db.Value.Agreements.Where(ea => ea.AccountLegalEntityId == accountLegalEntityId)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<EmployerAgreementStatus?> GetEmployerAgreementStatus(long agreementId)
    {
        return await _db.Value.Agreements.Where(x => x.Id == agreementId).Select(x => x.StatusId).SingleOrDefaultAsync();
    }

    public async Task SetAccountLegalEntityAgreementDetails(long accountLegalEntityId, long? pendingAgreementId, int? pendingAgreementVersion, long? signedAgreementId, int? signedAgreementVersion)
    {
        var legalEntity = await _db.Value.AccountLegalEntities.SingleAsync(x => x.Id == accountLegalEntityId);
        legalEntity.PendingAgreementId = pendingAgreementId;
        legalEntity.PendingAgreementVersion = pendingAgreementVersion;
        legalEntity.SignedAgreementId = signedAgreementId;
        legalEntity.SignedAgreementVersion = signedAgreementVersion;
        await _db.Value.SaveChangesAsync();
    }

    public async Task<AccountLegalEntity> GetOrganisationsAgreements(long accountLegalEntityId)
    {
        var accountLegalEntity = await _db.Value.AccountLegalEntities
            .Where(ale => ale.Id == accountLegalEntityId
                          && ale.Deleted == null
                          && ale.Agreements.Any(ea =>
                              ea.StatusId == EmployerAgreementStatus.Pending ||
                              ea.StatusId == EmployerAgreementStatus.Signed))
            .SingleOrDefaultAsync()
            .ConfigureAwait(false);

        return accountLegalEntity;
    }
}