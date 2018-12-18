using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Api.IntegrationTests.TestUtils.DataAccess
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