using SFA.DAS.EmployerFinance.Models;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EntityFramework;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Account;


namespace SFA.DAS.EmployerFinance.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerFinanceDbContext : DbContext
    {
        public virtual DbSet<HealthCheck> HealthChecks { get; set; }
        public virtual DbSet<PeriodEnd> PeriodEnds { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<TransactionLineEntity> Transactions { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }

        static EmployerFinanceDbContext()
        {
            Database.SetInitializer<EmployerFinanceDbContext>(null);
        }

        public EmployerFinanceDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public EmployerFinanceDbContext(DbConnection connection, DbTransaction transaction = null)
            : base(connection, false)
        {
            if (transaction == null) Database.BeginTransaction();
            else Database.UseTransaction(transaction);
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
            modelBuilder.Entity<HealthCheck>().ToTable("HealthChecks", "dbo");
            modelBuilder.Entity<TransactionLineEntity>().ToTable("TransactionLine");
            modelBuilder.Entity<TransactionLineEntity>().HasKey(t => t.Id);
            //TODO : check we need this : copied the below code from  SFA.DAS.EAS.Infrastructure.Data.EmployerFinanceDbContext 
            modelBuilder.Entity<Payment>().Ignore(a => a.StandardCode).Ignore(a => a.FrameworkCode).Ignore(a => a.ProgrammeType).Ignore(a => a.PathwayCode).Ignore(a => a.PathwayName);
            modelBuilder.Entity<Payment>().Property(a => a.EmployerAccountId).HasColumnName("AccountId");
            modelBuilder.Ignore<PaymentDetails>();
        }
    }
}