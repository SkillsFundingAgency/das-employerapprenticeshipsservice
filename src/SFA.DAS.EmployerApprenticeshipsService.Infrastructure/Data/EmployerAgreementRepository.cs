using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class EmployerAgreementRepository : BaseRepository, IEmployerAgreementRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public EmployerAgreementRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db) 
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<List<LegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId, bool signedOnly)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
                
            var sql = @"
                SELECT le.*
                FROM [employer_account].[LegalEntity] le
                WHERE le.Id IN (
	                SELECT LegalEntityId
	                FROM [employer_account].[EmployerAgreement] ea
	                WHERE ea.AccountId = @accountId";

            if (signedOnly)
            {
                sql += " AND ea.StatusId = 2";
            }
            else
            {
                sql += " AND ea.StatusId IN (1, 2)";
            }

            sql += ")";

            var result = await _db.Value.Database.Connection.QueryAsync<LegalEntity>(
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
            parameters.Add("@employerAgreementId", templateId, DbType.Int64,ParameterDirection.InputOutput);

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
    }
}