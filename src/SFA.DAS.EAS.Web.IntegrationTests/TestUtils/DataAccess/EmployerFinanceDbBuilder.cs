using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess
{
    class EmployerFinanceDbBuilder
    {
        private readonly EmployerFinanceDbContext _dbContext;

        public EmployerFinanceDbBuilder(
            DbBuilderDependentRepositories dependentRepositories,
            EmployerFinanceDbContext dbContext)
        {
            DependentRepositories = dependentRepositories;
            _dbContext = dbContext;
        }

        public DbBuilderDependentRepositories DependentRepositories { get; }
    }
}