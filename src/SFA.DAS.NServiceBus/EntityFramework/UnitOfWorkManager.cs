using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public class UnitOfWorkManager<T> : IUnitOfWorkManager where T : DbContext, IOutboxDbContext
    {
        private readonly IMessageSession _messageSession;
        private readonly IUnitOfWorkContext _unitOfWorkContext;
        private readonly Lazy<DbConnection> _connection;
        private readonly Lazy<T> _db;
        private DbTransaction _transaction;

        public UnitOfWorkManager(
            IMessageSession messageSession,
            IUnitOfWorkContext unitOfWorkContext,
            Lazy<DbConnection> connection,
            Lazy<T> db)
        {
            _messageSession = messageSession;
            _unitOfWorkContext = unitOfWorkContext;
            _connection = connection;
            _db = db;
        }

        public void Begin()
        {
            _connection.Value.TryOpenAsync().GetAwaiter().GetResult();
            _transaction = _connection.Value.BeginTransaction();
            _unitOfWorkContext.Set(_connection.Value);
            _unitOfWorkContext.Set(_transaction);
        }

        public void End(Exception ex = null)
        {
            using (_transaction)
            {
                if (ex == null)
                {
                    SaveChanges();
                    SaveOutboxMessage();
                    CommitTransaction();
                    SendProcessOutboxMessageCommand();
                }
            }
        }

        private void CommitTransaction()
        {
            _transaction.Commit();
        }

        private void SaveChanges()
        {
            _db.Value.SaveChanges();
        }

        private void SaveOutboxMessage()
        {
            var events = _unitOfWorkContext.GetEvents().ToList();

            if (events.Any())
            {
                var message = new OutboxMessage(events);

                _db.Value.OutboxMessages.Add(message);
                _db.Value.SaveChanges();
            }
        }

        private void SendProcessOutboxMessageCommand()
        {
            var outboxMessage = _db.Value.ChangeTracker.Entries<OutboxMessage>().Select(e => e.Entity).SingleOrDefault();

            if (outboxMessage != null)
            {
                var command = new ProcessOutboxMessageCommand();
                var options = new SendOptions();

                options.SetMessageId(outboxMessage.Id);
                _messageSession.Send(command, options).GetAwaiter().GetResult();
            }
        }
    }
}