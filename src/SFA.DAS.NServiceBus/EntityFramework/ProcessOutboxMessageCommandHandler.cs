using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public class ProcessOutboxMessageCommandHandler : IHandleMessages<ProcessOutboxMessageCommand>
    {
        private readonly Lazy<IOutboxDbContext> _db;

        public ProcessOutboxMessageCommandHandler(Lazy<IOutboxDbContext> db)
        {
            _db = db;
        }

        public async Task Handle(ProcessOutboxMessageCommand message, IMessageHandlerContext context)
        {
            var outboxMessageId = Guid.Parse(context.MessageId);
            var outboxMessage = await _db.Value.OutboxMessages.SingleAsync(m => m.Id == outboxMessageId);
            var events = outboxMessage.Publish();
            var tasks = events.Select(context.Publish);

            await Task.WhenAll(tasks);
        }
    }
}