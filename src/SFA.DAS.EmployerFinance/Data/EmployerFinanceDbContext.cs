using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EntityFramework;
using SFA.DAS.NServiceBus;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerFinanceDbContext : DbContext
    {
        public virtual DbSet<PeriodEnd> PeriodEnds { get; set; }

        static EmployerFinanceDbContext()
        {
            Database.SetInitializer<EmployerFinanceDbContext>(null);
        }

        public EmployerFinanceDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public EmployerFinanceDbContext(IUnitOfWorkContext unitOfWorkContext)
            : base(unitOfWorkContext.Get<DbConnection>(), false)
        {
            Database.UseTransaction(unitOfWorkContext.Get<DbTransaction>());
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
        }
    }
}