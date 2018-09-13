using SFA.DAS.EAS.Domain.Models.Payments;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerFinanceDbContext : DbContext
    {
        public virtual DbSet<PeriodEnd> PeriodEnds { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }

        static EmployerFinanceDbContext()
        {
            Database.SetInitializer<EmployerFinanceDbContext>(null);
        }

        public EmployerFinanceDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected EmployerFinanceDbContext()
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
            modelBuilder.Entity<Payment>().Ignore(a => a.StandardCode).Ignore(a => a.FrameworkCode).Ignore(a => a.ProgrammeType).Ignore(a => a.PathwayCode).Ignore(a => a.PathwayName);
            modelBuilder.Entity<Payment>().Property(a => a.EmployerAccountId).HasColumnName("AccountId");
            modelBuilder.Ignore<PaymentDetails>();
        }
    }
}