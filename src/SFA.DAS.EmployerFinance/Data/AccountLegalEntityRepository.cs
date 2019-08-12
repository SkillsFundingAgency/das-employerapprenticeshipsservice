using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration; //todo not totally convinced this is the right way to go
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class AccountLegalEntityRepository : BaseRepository, IAccountLegalEntityRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public AccountLegalEntityRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public Task CreateAccountLegalEntity(long id, DateTime deleted, long pendingAgreementId, long signedAgreementId,
            int signedAgreementVersion, long accountId, long legalEntityId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", id, DbType.Int64);
            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);
            parameters.Add("@signedAgreementVersion", signedAgreementVersion, DbType.Int32);
            parameters.Add("@signedAgreementId", signedAgreementId, DbType.Int64);
            parameters.Add("@pendingAgreementId", pendingAgreementId, DbType.Int64);
            parameters.Add("@deleted", deleted, DbType.DateTime);

            return _db.Value.Database.Connection.ExecuteAsync(
                "[employer_account].[CreateAccountLegalEntity]",
                parameters,
                _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}