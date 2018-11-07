using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EntityFramework;

namespace SFA.DAS.EmployerFinance.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerFinanceDbContextReadOnly : DbContext
    {
        public virtual DbSet<PeriodEnd> PeriodEnds { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Paye> Payees { get; set; }

        static EmployerFinanceDbContextReadOnly()
        {
            Database.SetInitializer<EmployerFinanceDbContext>(null);
        }

        public EmployerFinanceDbContextReadOnly(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.BeginTransaction();
        }

        protected EmployerFinanceDbContextReadOnly()
        {
        }

        public override int SaveChanges()
        {
            throw new Exception($"The {GetType().FullName} is for read only operations");
        }

        public override Task<int> SaveChangesAsync()
        {

            throw new Exception($"The {GetType().FullName} is for read only operations");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new Exception($"The {GetType().FullName} is for read only operations");
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