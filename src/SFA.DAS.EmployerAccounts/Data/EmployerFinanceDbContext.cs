using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Data
{
    //[DbConfigurationType(typeof(EntityFramework.SqlAzureDbConfiguration))]
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerFinanceDbContext : DbContext
    {
        static EmployerFinanceDbContext()
        {
            Database.SetInitializer<EmployerFinanceDbContext>(null);
        }

        public EmployerFinanceDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.BeginTransaction();
        }

        public EmployerFinanceDbContext(DbConnection connection, DbTransaction transaction = null)
            : base(connection, false)
        {
            if (transaction != null) Database.UseTransaction(transaction);
            else Database.BeginTransaction();
        }

        protected EmployerFinanceDbContext()
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
        }

        protected override void Dispose(bool disposing)
        {
            this.Database.Connection.Dispose();
            base.Dispose(disposing);
        }
    }
}