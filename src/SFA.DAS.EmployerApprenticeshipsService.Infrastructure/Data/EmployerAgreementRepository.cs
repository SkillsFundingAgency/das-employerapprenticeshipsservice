﻿using System.Collections.Generic;
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
        public EmployerAgreementRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger) 
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<List<LegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId, bool signedOnly)
        {
            var result = await WithConnection(async c =>
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

                return await c.QueryAsync<LegalEntity>(
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
                sql: "SELECT * FROM [employer_account].[EmployerAgreementTemplate] ORDER BY ReleasedDate DESC;",
                commandType: CommandType.Text));

            return result.FirstOrDefault();
        }
        
        public async Task RemoveLegalEntityFromAccount(long agreementId)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@employerAgreementId", agreementId, DbType.Int64);

                return await c.ExecuteAsync(
                    sql: "[employer_account].[RemoveLegalEntityFromAccount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                
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

        public async Task<int?> GetLatestSignedAgreementVersion(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryFirstAsync<int?>(
                    sql: "[employer_account].[GetLatestSignedAgreementVersionForAccount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result;
        }
    }
}