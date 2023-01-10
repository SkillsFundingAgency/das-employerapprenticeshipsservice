using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class AccountLegalEntityRepository : BaseRepository, IAccountLegalEntityRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public AccountLegalEntityRepository(EmployerAccountsConfiguration configuration, ILog logger,  Lazy<EmployerAccountsDbContext> db) 
            : base(configuration.SqlConnectionString, logger)
        {
            _db = db;
        }

        public async Task<List<AccountLegalEntity>> GetAccountLegalEntities(string accountHashedId)
        {
            var accountLegalEntities =  await _db.Value.AccountLegalEntities.Where(l =>
                l.Account.HashedId == accountHashedId &&
                (l.PendingAgreementId != null || l.SignedAgreementId != null) &&
                l.Deleted == null).ToListAsync();

            return accountLegalEntities;
        }
    }
}