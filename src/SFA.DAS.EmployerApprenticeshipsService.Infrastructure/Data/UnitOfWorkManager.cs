﻿using System.Data.Entity;
using System.Linq;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IMessagePublisher _messagePublisher;

        public UnitOfWorkManager(EmployerAccountDbContext db, IMessagePublisher messagePublisher)
        {
            _db = db;
            _messagePublisher = messagePublisher;
        }

        public void End()
        {
            _db.SaveChanges();

            foreach (var message in _db.ChangeTracker.Entries<IEntity>().SelectMany(e => e.Entity.GetEvents()))
            {
                _messagePublisher.PublishAsync(message);
            }

            foreach (var entry in _db.ChangeTracker.Entries().Where(e => e.Entity != null))
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}