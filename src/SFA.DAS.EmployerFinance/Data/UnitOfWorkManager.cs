using System;
using System.Data.Entity;

namespace SFA.DAS.EmployerFinance.Data
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private DbContextTransaction _transaction;

        public UnitOfWorkManager(Lazy<EmployerAccountsDbContext> db)
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