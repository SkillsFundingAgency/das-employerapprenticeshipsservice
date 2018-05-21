using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Payments;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerFinancialDbContext : DbContext
    {
        public virtual DbSet<PeriodEnd> PeriodEnds { get; set; }

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