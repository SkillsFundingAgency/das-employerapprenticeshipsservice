﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class EmployerAgreementRepository : BaseRepository, IEmployerAgreementRepository
    {
        public EmployerAgreementRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<List<AccountSpecificLegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId,
            bool signedOnly)
        {
            var result = await WithConnection(async c =>
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

                return await c.QueryAsync<AccountSpecificLegalEntity>(
                    sql: sql,
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }

        public async Task CreateEmployerAgreementTemplate(string templateRef, string text)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ref", templateRef, DbType.String);
                parameters.Add("@text", text, DbType.String);

                var trans = c.BeginTransaction();
                var result = await c.ExecuteAsync(
                    sql: "[employer_account].[CreateEmployerAgreementTemplate]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);
                trans.Commit();
                return result;
            });
        }

        public async Task<long> CreateEmployerAgreeement(int templateId, long accountId, long legalEntityId)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@templateId", templateId, DbType.Int32);
                parameters.Add("@employerAgreementId", templateId, DbType.Int64, ParameterDirection.InputOutput);

                var trans = c.BeginTransaction();
                var result = await c.ExecuteAsync(
                    sql: "[employer_account].[CreateEmployerAgreement]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);
                trans.Commit();

                var newAgreementId = parameters.Get<long>("@employerAgreementId");

                return newAgreementId;
            });
        }

        public async Task<EmployerAgreementView> GetEmployerAgreement(long agreementId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@agreementId", agreementId, DbType.Int64);

                return await c.QueryAsync<EmployerAgreementView>(
                    sql: "[employer_account].[GetEmployerAgreement]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }

        public async Task SignAgreement(SignEmployerAgreement agreement)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@agreementId", agreement.AgreementId, DbType.Int64);
                parameters.Add("@signedById", agreement.SignedById, DbType.Int64);
                parameters.Add("@signedByName", agreement.SignedByName, DbType.String);
                parameters.Add("@signedDate", agreement.SignedDate, DbType.DateTime);

                var result = await c.ExecuteAsync(
                    sql: "[employer_account].[SignEmployerAgreement]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            });
        }

        public async Task<EmployerAgreementTemplate> GetEmployerAgreementTemplate(int templateId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@templateId", templateId, DbType.Int32);

                return await c.QueryAsync<EmployerAgreementTemplate>(
                    sql: "SELECT * FROM [employer_account].[EmployerAgreementTemplate] WHERE Id = @templateId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task<EmployerAgreementTemplate> GetLatestAgreementTemplate()
        {
            var result = await WithConnection(async c => await c.QueryAsync<EmployerAgreementTemplate>(
                sql: "SELECT TOP(1) * FROM [employer_account].[EmployerAgreementTemplate] ORDER BY VersionNumber DESC;",
                commandType: CommandType.Text));

            return result.FirstOrDefault();
        }

        public async Task RemoveLegalEntityFromAccount(long agreementId)
        {
            await WithTransaction(async (c, txn) =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@employerAgreementId", agreementId, DbType.Int64);

                return await c.ExecuteAsync(
                    sql: "[employer_account].[RemoveLegalEntityFromAccount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: txn);

            });
        }

        public async Task<List<RemoveEmployerAgreementView>> GetEmployerAgreementsToRemove(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<RemoveEmployerAgreementView>(
                    sql: "[employer_account].[GetEmployerAgreementsToRemove_ByAccountId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public Task EvaluateEmployerLegalEntityAgreementStatus(long accountId, long legalEntityId)
        {
            return WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);

                return await connection.ExecuteAsync(
                    sql: "[employer_account].[EvaluateEmployerLegalEntityAgreementStatus]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<AccountLegalEntityModel> GetAccountLegalEntity(long accountLegalEntityId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", accountLegalEntityId, DbType.Int64);

                return await c.QueryAsync<AccountLegalEntityModel>(
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
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }
    }
}