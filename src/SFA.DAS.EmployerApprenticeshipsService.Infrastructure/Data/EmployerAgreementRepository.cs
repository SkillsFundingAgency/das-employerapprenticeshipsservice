using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class EmployerAgreementRepository : BaseRepository, IEmployerAgreementRepository
    {
        public EmployerAgreementRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger) 
            : base(configuration, logger)
        {
        }

        public async Task<List<LegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<LegalEntity>(
                    sql: "GetLegalEntitiesLinkedToAccount",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public async Task CreateEmployerAgreementTemplate(string text)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@text", text, DbType.String);

                var trans = c.BeginTransaction();
                var result = await c.ExecuteAsync(
                    sql: "[dbo].[CreateEmployerAgreementTemplate]",
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
                    sql: "[dbo].[GetEmployerAgreement]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }

        public async Task SignAgreement(long agreementId, string externalUserId, string signedByName)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@agreementId", agreementId, DbType.Int64);
                parameters.Add("@signedById", Guid.Parse(externalUserId), DbType.Guid);
                parameters.Add("@signedByName", signedByName, DbType.String);

                var result = await c.ExecuteAsync(
                    sql: "[dbo].[SignEmployerAgreement]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            });
        }
    }
}