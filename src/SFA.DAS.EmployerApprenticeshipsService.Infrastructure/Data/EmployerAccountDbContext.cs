﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerAccountDbContext : DbContext
    {
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<EmployerAgreement> Agreements { get; set; }
        public virtual DbSet<AgreementTemplate> AgreementTemplates { get; set; }
        public virtual DbSet<LegalEntity> LegalEntities { get; set; }
        public virtual DbSet<AccountLegalEntity> AccountLegalEntity { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<TransferConnectionInvitation> TransferConnectionInvitations { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAccountSetting> UserAccountSettings { get; set; }
        public virtual DbSet<Paye> Payees { get; set; }

        static EmployerAccountDbContext()
        {
            Database.SetInitializer<EmployerAccountDbContext>(null);
        }

        public EmployerAccountDbContext(EmployerApprenticeshipsServiceConfiguration config)
            : base(config.DatabaseConnectionString)
        {
        }

        protected EmployerAccountDbContext()
        {
        }

        public virtual Task<List<T>> SqlQueryAsync<T>(string query, params object[] parameters)
        {
            return Database.SqlQuery<T>(query, parameters).ToListAsync();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Account>().Ignore(a => a.RoleId).Ignore(a => a.RoleName);
            modelBuilder.Entity<Account>().HasMany(a => a.AccountLegalEntities);
            modelBuilder.Entity<Account>().HasMany(a => a.ReceivedTransferConnectionInvitations).WithRequired(i => i.ReceiverAccount);
            modelBuilder.Entity<Account>().HasMany(a => a.SentTransferConnectionInvitations).WithRequired(i => i.SenderAccount);
            modelBuilder.Entity<AgreementTemplate>().ToTable("EmployerAgreementTemplate").HasMany(t => t.Agreements);
            modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.AccountLegalEntity);
            modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.Template);

            modelBuilder.Entity<AccountLegalEntity>().HasMany<EmployerAgreement>(ale => ale.Agreements);
            modelBuilder.Entity<AccountLegalEntity>().HasRequired(ale => ale.Account);
            modelBuilder.Entity<AccountLegalEntity>().HasRequired(ale => ale.LegalEntity);
            modelBuilder.Entity<AccountLegalEntity>().HasOptional(ale => ale.SignedAgreement).WithMany().HasForeignKey(ale => ale.SignedAgreementId);
            modelBuilder.Entity<AccountLegalEntity>().HasOptional(ale => ale.PendingAgreement).WithMany().HasForeignKey(ale => ale.PendingAgreementId);

            //modelBuilder.Entity<Course>()
            //    .HasRequired(c => c.Department)
            //    .WithMany(d => d.Courses)
            //    .HasForeignKey(c => c.SomeDepartmentID);

            modelBuilder.Entity<Membership>().HasKey(m => new { m.AccountId, m.UserId }).Ignore(m => m.RoleId).Property(m => m.Role).HasColumnName(nameof(Membership.RoleId));
            modelBuilder.Entity<User>().Ignore(u => u.FullName).Ignore(u => u.UserRef).Property(u => u.ExternalId).HasColumnName(nameof(User.UserRef));
            modelBuilder.Entity<TransferConnectionInvitation>().HasRequired(i => i.ReceiverAccount);
            modelBuilder.Entity<TransferConnectionInvitation>().HasRequired(i => i.SenderAccount);
            modelBuilder.Entity<UserAccountSetting>().HasRequired(u => u.Account);
            modelBuilder.Entity<UserAccountSetting>().HasRequired(u => u.User);
            modelBuilder.Entity<UserAccountSetting>().ToTable("UserAccountSettings");
            modelBuilder.HasDefaultSchema("employer_account");
            modelBuilder.Entity<Paye>().Ignore(a => a.AccountId);
        }
    }
}