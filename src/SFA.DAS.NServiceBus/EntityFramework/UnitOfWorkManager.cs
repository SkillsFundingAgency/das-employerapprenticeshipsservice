using System;
using System.Data.Common;
using System.Linq;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly IMessageSession _messageSession;
        private readonly IUnitOfWorkContext _unitOfWorkContext;
        private readonly Lazy<DbConnection> _connection;
        private readonly Lazy<IOutboxDbContext> _db;
        private DbTransaction _transaction;

        public UnitOfWorkManager(
            IMessageSession messageSession,
            IUnitOfWorkContext unitOfWorkContext,
            Lazy<DbConnection> connection,
            Lazy<IOutboxDbContext> db)
        {
            _messageSession = messageSession;
            _unitOfWorkContext = unitOfWorkContext;
            _connection = connection;
            _db = db;
        }

        public void Begin()
        {
            OpenConnection();
            BeginTransaction();
            SetUnitOfWorkContext();
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

        private void BeginTransaction()
        {
            _transaction = _connection.Value.BeginTransaction();
        }

        private void CommitTransaction()
        {
            _transaction.Commit();
        }

        private void OpenConnection()
        {
            try
            {
                _connection.Value.Open();
            }
            catch
            {
                _connection.Value.Dispose();
                throw;
            }
        }

        private void SaveChanges()
        {
            _db.Value.SaveChangesAsync().GetAwaiter().GetResult();
        }

        private void SaveOutboxMessage()
        {
            var events = _unitOfWorkContext.GetEvents().ToList();

            if (events.Any())
            {
                var outboxMessage = new OutboxMessage(events);

                _db.Value.OutboxMessages.Add(outboxMessage);
                _db.Value.SaveChangesAsync().GetAwaiter().GetResult();
            }
        }

        private void SendProcessOutboxMessageCommand()
        {
            var outboxMessage = _db.Value.OutboxMessages.Local.SingleOrDefault();

            if (outboxMessage != null)
            {
                var options = new SendOptions();
                
                options.SetMessageId(outboxMessage.Id.ToString());

                _messageSession.Send(new ProcessOutboxMessageCommand(), options).GetAwaiter().GetResult();
            }
        }

        private void SetUnitOfWorkContext()
        {
            _unitOfWorkContext.Set(_connection.Value);
            _unitOfWorkContext.Set(_transaction);
        }
    }
}