using System;
using System.Data.Entity;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;
        private DbContextTransaction _transaction;

        public UnitOfWorkManager(Lazy<EmployerFinanceDbContext> db)
        {
            _db = db;
        }

        public void Begin()
        {
            _transaction = _db.Value.Database.BeginTransaction();
        }

        public void End(Exception ex = null)
        {
            using (_transaction)
            {
                if (ex == null)
                {
                    _db.Value.SaveChanges();
                    _transaction.Commit();
                }
            }
        }
    }
}