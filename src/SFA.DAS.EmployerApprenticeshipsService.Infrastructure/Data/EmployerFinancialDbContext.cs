using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Payments;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerFinancialDbContext : DbContext
    {
        public virtual DbSet<PeriodEnd> PeriodEnds { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }

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
            modelBuilder.Entity<Payment>().Ignore(a => a.StandardCode).Ignore(a => a.FrameworkCode)
                .Ignore(a => a.ProgrammeType).Ignore(a => a.PathwayCode).Ignore(a => a.PathwayName);
            modelBuilder.Entity<Payment>().Property(a => a.EmployerAccountId).HasColumnName("AccountId");
            modelBuilder.Ignore<PaymentDetails>();
            modelBuilder.HasDefaultSchema("employer_financial");
        }
    }
}