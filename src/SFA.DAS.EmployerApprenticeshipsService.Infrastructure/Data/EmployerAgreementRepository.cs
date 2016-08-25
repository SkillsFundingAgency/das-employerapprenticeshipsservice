using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class EmployerAgreementRepository : BaseRepository, IEmployerAgreementRepository
    {
        public EmployerAgreementRepository(EmployerApprenticeshipsServiceConfiguration configuration) 
            : base(configuration)
        {
        }

        public async Task<List<LegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<LegalEntity>(
                    sql: "[account].[GetLegalEntitiesLinkedToAccount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
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
                    sql: "[account].[CreateEmployerAgreementTemplate]",
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
                    sql: "[account].[GetEmployerAgreement]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }

        public async Task SignAgreement(long agreementId, long signedById, string signedByName)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@agreementId", agreementId, DbType.Int64);
                parameters.Add("@signedById", signedById, DbType.Int64);
                parameters.Add("@signedByName", signedByName, DbType.String);

                var result = await c.ExecuteAsync(
                    sql: "[account].[SignEmployerAgreement]",
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
                    sql: "[account].[ReleaseEmployerAgreementTemplate]",
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
                    sql: "SELECT * FROM [account].[EmployerAgreementTemplate] WHERE Id = @templateId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }
    }
}