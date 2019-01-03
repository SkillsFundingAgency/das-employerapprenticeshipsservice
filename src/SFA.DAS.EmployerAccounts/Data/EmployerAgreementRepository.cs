using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.Organisation;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.Data
{
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
	                        ale.Id as AccountLegalEntityId, ale.Name, ale.Address, ale.SignedAgreementVersion, ale.SignedAgreementId, ale.PendingAgreementVersion, ale.PendingAgreementId, ale.PublicHashedId as AccountLegalEntityPublicHashedId
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

        public Task CreateEmployerAgreementTemplate(string templateRef, string text)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ref", templateRef, DbType.String);
            parameters.Add("@text", text, DbType.String);
            
            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[CreateEmployerAgreementTemplate]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<long> CreateEmployerAgreeement(int templateId, long accountId, long legalEntityId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);
            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@templateId", templateId, DbType.Int32);
            parameters.Add("@employerAgreementId", templateId, DbType.Int64, ParameterDirection.InputOutput);

            await _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[CreateEmployerAgreement]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            var newAgreementId = parameters.Get<long>("@employerAgreementId");

            return newAgreementId;
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

        public async Task<EmployerAgreementTemplate> GetEmployerAgreementTemplate(int templateId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@templateId", templateId, DbType.Int32);

            var result = await _db.Value.Database.Connection.QueryAsync<EmployerAgreementTemplate>(
                sql: "SELECT * FROM [employer_account].[EmployerAgreementTemplate] WHERE Id = @templateId;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public async Task<EmployerAgreementTemplate> GetLatestAgreementTemplate()
        {
            var result = await _db.Value.Database.Connection.QueryAsync<EmployerAgreementTemplate>(
                sql: "SELECT TOP(1) * FROM [employer_account].[EmployerAgreementTemplate] ORDER BY VersionNumber DESC;",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.FirstOrDefault();
        }

        public async Task<EmployerAgreementRemoved[]> RemoveLegalEntityFromAccount(long accountLegalEntityId)
        {
            var accountLegalEntity = await _db.Value.AccountLegalEntities
                                                .Include(ale => ale.Agreements)
                                                .SingleAsync(ale => ale.Id == accountLegalEntityId);

            accountLegalEntity.Deleted = DateTime.UtcNow;

            var affectedAgreements = accountLegalEntity.Agreements
                    .Where(agreement => agreement.StatusId != EmployerAgreementStatus.Removed) 
                    .ToArray();

            var result = affectedAgreements
                .Select(agreement => new EmployerAgreementRemoved
                {
                    EmployerAgreementId = agreement.Id,
                    LegalEntityId = accountLegalEntity.LegalEntityId,
                    AccountLegalEntityId = accountLegalEntity.Id,
                    PreviousStatus = agreement.StatusId,
                    Signed = agreement.StatusId == EmployerAgreementStatus.Signed
                })
                .ToArray();
            
            foreach(var agreement in affectedAgreements)
            {
                agreement.StatusId = EmployerAgreementStatus.Removed;
            }

            await _db.Value.SaveChangesAsync();

            return result;
        }

        public async Task<List<RemoveEmployerAgreementView>> GetEmployerAgreementsToRemove(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<RemoveEmployerAgreementView>(
                sql: "[employer_account].[GetEmployerAgreementsToRemove_ByAccountId]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public Task EvaluateEmployerLegalEntityAgreementStatus(long accountId, long legalEntityId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[EvaluateEmployerLegalEntityAgreementStatus]",
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
                SELECT  ALE.AccountId,
                        ALE.Id as AccountLegalEntityId, 
                        LE.Id AS LegalEntityId, 
                        ALE.Name, 
                        ALE.PublicHashedId AS AccountLegalEntityPublicHashedId, 
                        ALE.Name, 
                        ALE.Address, 
                        LE.Source AS OrganisationType, 
                        LE.Code AS Identifier,
                        ALE.SignedAgreementId
                FROM    [employer_account].[AccountLegalEntity] AS ALE 
                        JOIN [employer_account].[LegalEntity] AS LE
                            ON LE.Id = ALE.LegalEntityId
                    WHERE ALE.Id = @Id;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }
    }
}