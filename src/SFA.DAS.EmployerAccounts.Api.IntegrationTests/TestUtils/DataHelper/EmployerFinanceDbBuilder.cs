using SFA.DAS.EmployerAccounts.Data;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper
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

        public DbBuilderContext Context { get;  } = new DbBuilderContext();

        public DbBuilderDependentRepositories DependentRepositories { get; }
    }
}