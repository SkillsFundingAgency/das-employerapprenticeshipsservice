using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.MarkerInterfaces;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class EmployerAccountRepository : BaseRepository, IEmployerAccountRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;
        private readonly IHashingService _hashingService;
        private readonly IPublicHashingService _publicHashingService;

        public EmployerAccountRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db,
            IHashingService hashingService, IPublicHashingService publicHashingService)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
            _hashingService = hashingService;
            _publicHashingService = publicHashingService;
        }

        public async Task<Account> Get(long id)
        {
            var account = await _db.Value.Accounts.SingleOrDefaultAsync(a => a.Id == id);
            return account;
        }

        public async Task<List<Account>> Get(List<long> accountIds)
        {
            var accounts = await _db.Value.Accounts.Where(a => accountIds.Contains(a.Id)).ToListAsync();
            return accounts;
        }

        public async Task<Account> Get(string publicHashedId)
        {
            var account = _publicHashingService.TryDecodeValue(publicHashedId, out var accountId)
                ? await Get(accountId)
                : null;
            
            return account;
        }

        public async Task<List<Account>> GetAll()
        {
            var accounts = await _db.Value.Accounts.ToListAsync();
            return accounts;
        }
    }
}
