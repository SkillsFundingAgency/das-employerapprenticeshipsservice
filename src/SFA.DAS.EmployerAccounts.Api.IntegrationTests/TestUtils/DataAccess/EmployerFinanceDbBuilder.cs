using SFA.DAS.EmployerAccounts.Data;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess
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