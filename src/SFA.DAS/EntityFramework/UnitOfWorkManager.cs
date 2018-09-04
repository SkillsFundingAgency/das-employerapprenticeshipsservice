using System;
using System.Data.Entity;

namespace SFA.DAS.EntityFramework
{
    public class UnitOfWorkManager<T> : IUnitOfWorkManager where T : DbContext
    {
        private readonly Lazy<T> _db;
        private DbContextTransaction _transaction;

        public UnitOfWorkManager(Lazy<T> db)
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