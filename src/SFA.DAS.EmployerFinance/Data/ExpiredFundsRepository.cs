using System;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class ExpiredFundsRepository : BaseRepository, IExpiredFundsRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public ExpiredFundsRepository(string connectionString, ILog logger, Lazy<EmployerFinanceDbContext> db) : base(connectionString, logger)
        {
            _db = db;
        }
    }
}
