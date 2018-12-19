using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Data
{
    [DbConfigurationType(typeof(EntityFramework.SqlAzureDbConfiguration))]
    public class JobDbContext : DbContext
    {
        public virtual DbSet<Job> Jobs { get; set; }

        static JobDbContext()
        {
            Database.SetInitializer<JobDbContext>(null);
        }

        public JobDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.BeginTransaction();
        }

        protected JobDbContext()
        {
        }

        public virtual Task<List<T>> SqlQueryAsync<T>(string query, params object[] parameters)
        {
            return Database.SqlQuery<T>(query, parameters).ToListAsync();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema("employer_account");
            modelBuilder.Entity<Job>().Ignore(a => a.Id);
            modelBuilder.Entity<Job>().HasRequired(j => j.Name);
        }
    }
}