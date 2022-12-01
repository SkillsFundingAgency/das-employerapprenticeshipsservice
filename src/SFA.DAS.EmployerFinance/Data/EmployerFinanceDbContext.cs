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
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.EmployerFinance.Models.UserProfile;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerFinance.MarkerInterfaces;

namespace SFA.DAS.EmployerFinance.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerFinanceDbContext : DbContext
    {
        private readonly IHashingService _hashingService;
        private readonly IPublicHashingService _publicHashingService;

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<HealthCheck> HealthChecks { get; set; }
        public virtual DbSet<PeriodEnd> PeriodEnds { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<TransactionLineEntity> Transactions { get; set; }
        public virtual DbSet<TransferConnectionInvitation> TransferConnectionInvitations { get; set; }
        public virtual DbSet<User> Users { get; set; }

        static EmployerFinanceDbContext()
        {
            Database.SetInitializer<EmployerFinanceDbContext>(null);
        }

        public EmployerFinanceDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public EmployerFinanceDbContext(DbConnection connection, IHashingService hashingService, 
            IPublicHashingService publicHashingService, DbTransaction transaction = null)
            : base(connection, false)
        {
            _hashingService = hashingService;
            _publicHashingService = publicHashingService;

            ((IObjectContextAdapter)this).ObjectContext
                .ObjectMaterialized += OnObjectMaterialized;

            if (transaction != null) 
                Database.UseTransaction(transaction);
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
            modelBuilder.Entity<Account>().HasMany(a => a.ReceivedTransferConnectionInvitations).WithRequired(i => i.ReceiverAccount);
            modelBuilder.Entity<Account>().HasMany(a => a.SentTransferConnectionInvitations).WithRequired(i => i.SenderAccount);
            modelBuilder.Entity<Account>().Ignore(a => a.HashedId).Ignore(a => a.PublicHashedId);
            modelBuilder.Entity<HealthCheck>().ToTable("HealthChecks", "dbo");
            modelBuilder.Entity<Payment>().Ignore(a => a.StandardCode).Ignore(a => a.FrameworkCode).Ignore(a => a.ProgrammeType).Ignore(a => a.PathwayCode).Ignore(a => a.PathwayName);
            modelBuilder.Entity<Payment>().Property(a => a.EmployerAccountId).HasColumnName("AccountId");
            modelBuilder.Ignore<PaymentDetails>();
            modelBuilder.Entity<TransactionLineEntity>().ToTable("TransactionLine");
            modelBuilder.Entity<TransactionLineEntity>().HasKey(t => t.Id);
            modelBuilder.Entity<TransferConnectionInvitation>().HasRequired(i => i.ReceiverAccount);
            modelBuilder.Entity<TransferConnectionInvitation>().HasRequired(i => i.SenderAccount);
            modelBuilder.Entity<User>().Ignore(u => u.FullName).Ignore(u => u.UserRef).Property(u => u.Ref).HasColumnName(nameof(User.UserRef));
        }

        private void OnObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity as Entity;
            if (entity != null)
            {
                entity.HashingService = _hashingService;
                entity.PublicHashingService = _publicHashingService;
            }
        }
    }
}