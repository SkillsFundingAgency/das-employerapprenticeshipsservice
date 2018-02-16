using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Data
{
    public class EmployerAccountDbContext : DbContext
    {
        public virtual DbSet<Domain.Data.Entities.Account.Account> Accounts { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<TransferConnectionInvitation> TransferConnectionInvitations { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public EmployerAccountDbContext(string connectionString) : base(connectionString)
        {
        }

        protected EmployerAccountDbContext()
        {
        }

        public virtual IEnumerable<T> SqlQuery<T>(string query, params object[] parameters)
        {
            return Database.SqlQuery<T>(query, parameters).AsEnumerable();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema("employer_account");

            modelBuilder.Entity<Domain.Data.Entities.Account.Account>()
                .Ignore(a => a.RoleId)
                .Ignore(a => a.RoleName);

            modelBuilder.Entity<Membership>()
                .HasKey(m => new { m.AccountId, m.UserId })
                .Ignore(m => m.RoleId)
                .Property(m => m.Role).HasColumnName(nameof(Membership.RoleId));

            modelBuilder.Entity<User>()
                .Ignore(u => u.FullName)
                .Ignore(u => u.UserRef)
                .Property(u => u.ExternalId).HasColumnName(nameof(User.UserRef));
        }
    }
}