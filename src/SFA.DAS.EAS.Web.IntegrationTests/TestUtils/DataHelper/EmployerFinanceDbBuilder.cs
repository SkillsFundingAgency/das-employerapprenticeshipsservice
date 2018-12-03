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

        public bool HasTransaction()
        {
            
            return (_dbContext.Database.CurrentTransaction != null);
        }

        public EmployerFinanceDbBuilder BeginTransaction()
        {
            _dbContext.Database.BeginTransaction();
            return this;
        }

        public EmployerFinanceDbBuilder EnsureTransaction()
        {
            if (_dbContext.Database.CurrentTransaction == null)
                _dbContext.Database.BeginTransaction();

            return this;
        }

        public EmployerFinanceDbBuilder CommitTransaction()
        {
            if (_dbContext.Database.CurrentTransaction == null) return this;

            _dbContext.Database.CurrentTransaction.Commit();
            return this;
        }
    }
}