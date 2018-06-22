using NServiceBus;
using SFA.DAS.EAS.Domain.Models;
using System.Data.Entity;
using System.Linq;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IEndpointInstance _endpoint;


        public UnitOfWorkManager(EmployerAccountDbContext db, IEndpointInstance endpoint)
        {
            _db = db;
            _endpoint = endpoint;

        }

        public void End()
        {
            _db.SaveChanges();

            foreach (var message in _db.ChangeTracker.Entries<IEntity>().SelectMany(e => e.Entity.GetEvents()))
            {
                _endpoint.Publish(message);
            }

            foreach (var entry in _db.ChangeTracker.Entries().Where(e => e.Entity != null))
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}