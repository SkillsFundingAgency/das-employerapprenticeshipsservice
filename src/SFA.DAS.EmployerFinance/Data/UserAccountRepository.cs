using System;
using System.Data.Entity;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.UserProfile;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class UserAccountRepository : BaseRepository, IUserAccountRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public UserAccountRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public Task<User> Get(Guid @ref)
        {
            return _db.Value.Users.SingleOrDefaultAsync(u => u.Ref == @ref);
        }
    }
}
