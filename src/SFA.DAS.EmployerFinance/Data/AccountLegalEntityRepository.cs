using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class AccountLegalEntityRepository : BaseRepository, IAccountLegalEntityRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public AccountLegalEntityRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public Task CreateAccountLegalEntity(long id, long? pendingAgreementId, long? signedAgreementId,
            int? signedAgreementVersion, long accountId, long legalEntityId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", id, DbType.Int64);
            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);
            parameters.Add("@signedAgreementVersion", signedAgreementVersion, DbType.Int32);
            parameters.Add("@signedAgreementId", signedAgreementId, DbType.Int64);
            parameters.Add("@pendingAgreementId", pendingAgreementId, DbType.Int64);

            return _db.Value.Database.Connection.ExecuteAsync(
                "[employer_financial].[CreateAccountLegalEntity]",
                parameters,
                _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public Task SignAgreement(long signedAgreementId, int signedAgreementVersion, long accountId, long legalEntityId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);
            parameters.Add("@signedAgreementVersion", signedAgreementVersion, DbType.Int32);
            parameters.Add("@signedAgreementId", signedAgreementId, DbType.Int64);

            return _db.Value.Database.Connection.ExecuteAsync(
                "[employer_financial].[SignAccountLegalEntityAgreement]",
                parameters,
                _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public Task RemoveAccountLegalEntity(long id)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", id, DbType.Int64);

            return _db.Value.Database.Connection.ExecuteAsync(
                "[employer_financial].[RemoveAccountLegalEntity]",
                parameters,
                _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}