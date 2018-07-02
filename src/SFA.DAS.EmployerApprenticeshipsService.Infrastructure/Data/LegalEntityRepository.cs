using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class LegalEntityRepository : BaseRepository, ILegalEntityRepository
    {
        private readonly Lazy<EmployerAccountDbContext> _db;

        public LegalEntityRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<long[]> GetLegalEntitiesWithoutSpecificAgreement(long firstId, int count, int agreementId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@firstId", firstId, DbType.Int64);
            parameters.Add("@count", count, DbType.Int32);
            parameters.Add("@agreementId", agreementId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<long>(
                sql: "[employer_account].[GetLegalEntities_WithoutSpecificAgreement]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToArray();
        }

        public async Task<long[]> GetAccountsLinkedToLegalEntity(long legalEntityId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<long>(
                sql: "[employer_account].[GetAccountsLinkedToLegalEntity]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToArray();
        }

        public async Task<long[]> GetAccountsLinkedToLegalEntityWithoutSpecificAgreement(long legalEntityId, int templateId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);
            parameters.Add("@withoutAgreementVersion", templateId, DbType.Int32);

            var result = await _db.Value.Database.Connection.QueryAsync<long>(
                sql: "[employer_account].[GetAccountsLinkedToLegalEntity]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToArray();
        }
    }
}