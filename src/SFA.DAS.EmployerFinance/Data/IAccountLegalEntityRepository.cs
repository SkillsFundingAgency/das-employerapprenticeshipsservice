using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Configuration; //not totally convinced this is the right way to go
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IAccountLegalEntityRepository
    {
        Task CreateAccountLegalEntity(long id, bool deleted, long pendingAgreementId, long signedAgreementId, int signedAgreementVersion, long accountId);
    }

    public class AccountLegalEntityRepository : BaseRepository, IAccountLegalEntityRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public AccountLegalEntityRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public Task CreateAccountLegalEntity(long id, bool deleted, long pendingAgreementId, long signedAgreementId,
            int signedAgreementVersion, long accountId)
        {
            throw new System.NotImplementedException(); //todo here
        }
    }
}