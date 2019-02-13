using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.LevyAnalyser.Repositories
{
    public class AccountDbContext : DbContext 
    {
        public AccountDbContext(DbConnection connection) : base(connection, true)
        {
            // just call base    
        }

        public Task<long[]> GetAllAccountIdsAsync()
        {
            return Database.SqlQuery<long>(
                    $"SELECT Id FROM [employer_account].Account ORDER BY Id")
                .ToArrayAsync();
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema("employer_account");
        }
    }
}