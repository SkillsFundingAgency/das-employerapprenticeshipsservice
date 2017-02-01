using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class EmployerAgreementRepository : BaseRepository, IEmployerAgreementRepository
    {
        public EmployerAgreementRepository(EmployerApprenticeshipsServiceConfiguration configuration) 
            : base(configuration)
        {
        }

        public async Task<List<LegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId, bool signedOnly)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                var sql = "SELECT le.* FROM[employer_account].[LegalEntity] le INNER JOIN[employer_account].[EmployerAgreement] ea ON ea.LegalEntityId = le.Id INNER JOIN[employer_account].[AccountEmployerAgreement] aea ON aea.EmployerAgreementId = ea.Id WHERE aea.AccountId = @accountId";

                if (signedOnly)
                    sql += " AND ea.StatusId = 2";

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

        public async Task SignAgreement(long agreementId, long signedById, string signedByName, DateTime signedDate)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@agreementId", agreementId, DbType.Int64);
                parameters.Add("@signedById", signedById, DbType.Int64);
                parameters.Add("@signedByName", signedByName, DbType.String);
                parameters.Add("@signedDate", signedDate, DbType.DateTime);

                var result = await c.ExecuteAsync(
                    sql: "[employer_account].[SignEmployerAgreement]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            });
        }

        public async Task ReleaseEmployerAgreementTemplate(int templateId)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@templateId", templateId, DbType.Int32);

                var trans = c.BeginTransaction();
                var result = await c.ExecuteAsync(
                    sql: "[employer_account].[ReleaseEmployerAgreementTemplate]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);
                trans.Commit();
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
    }
}