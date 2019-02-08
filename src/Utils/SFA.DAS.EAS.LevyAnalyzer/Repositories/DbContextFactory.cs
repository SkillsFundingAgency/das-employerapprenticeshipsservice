using SFA.DAS.EAS.LevyAnalyser.Interfaces;

namespace SFA.DAS.EAS.LevyAnalyser.Repositories
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DbContextFactory(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public FinanceDbContext GetFinanceDbContext()
        {
            var connection = _dbConnectionFactory.GetConnection("Finance");
            return new FinanceDbContext(connection);
        }

        public AccountDbContext GetAccountDbContext()
        {
            var connection = _dbConnectionFactory.GetConnection("Account");
            return new AccountDbContext(connection);
        }
    }
}