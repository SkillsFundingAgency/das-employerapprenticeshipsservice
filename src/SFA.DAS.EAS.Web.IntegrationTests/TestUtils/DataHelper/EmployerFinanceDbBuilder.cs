using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper
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