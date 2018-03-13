using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class EmployerFinancialDbContext : DbContext
    {
        static EmployerFinancialDbContext()
        {
            Database.SetInitializer<EmployerFinancialDbContext>(null);
        }

        public EmployerFinancialDbContext(LevyDeclarationProviderConfiguration config)
            : base(config.DatabaseConnectionString)
        {
        }

        protected EmployerFinancialDbContext()
        {
        }

        public virtual Task<List<T>> SqlQueryAsync<T>(string query, params object[] parameters)
        {
            return Database.SqlQuery<T>(query, parameters).ToListAsync();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema("employer_financial");
        }
    }
}