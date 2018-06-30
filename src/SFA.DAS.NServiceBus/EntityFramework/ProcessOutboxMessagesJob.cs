using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public class ProcessOutboxMessagesJob : IProcessOutboxMessagesJob
    {
        private readonly IMessageSession _messageSession;
        private readonly Lazy<IOutboxDbContext> _db;

        public ProcessOutboxMessagesJob(IMessageSession messageSession, Lazy<IOutboxDbContext> db)
        {
            _messageSession = messageSession;
            _db = db;
        }

        public async Task RunAsync()
        {
            var outboxMessageIds = await _db.Value.OutboxMessages
                .Where(m => m.Sent <= DbFunctions.AddMinutes(DateTime.UtcNow, -10) && m.Published == null)
                .OrderBy(m => m.Id)
                .Select(m => m.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            var tasks = outboxMessageIds.Select(i =>
            {
                var options = new SendOptions();
                
                options.SetMessageId(i);

                return _messageSession.Send(new ProcessOutboxMessageCommand(), options);
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}