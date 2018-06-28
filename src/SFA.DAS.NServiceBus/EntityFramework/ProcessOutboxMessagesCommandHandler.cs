using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public class ProcessOutboxMessagesCommandHandler : IHandleMessages<ProcessOutboxMessagesCommand>
    {
        private readonly Lazy<IOutboxDbContext> _db;

        public ProcessOutboxMessagesCommandHandler(Lazy<IOutboxDbContext> db)
        {
            _db = db;
        }

        public async Task Handle(ProcessOutboxMessagesCommand message, IMessageHandlerContext context)
        {
            List<string> outboxMessageIds;

            while ((outboxMessageIds = await _db.Value.OutboxMessages
                    .Where(m => m.Created <= DbFunctions.AddMinutes(DateTime.UtcNow, 10) && m.Dispatched == null)
                    .Take(100)
                    .Select(m => m.Id)
                    .ToListAsync())
                .Any())
            {
                var tasks = outboxMessageIds.Select(i =>
                {
                    var options = new SendOptions();

                    options.RouteToThisEndpoint();
                    options.SetMessageId(i);

                    return context.Send(new ProcessOutboxMessageCommand(), options);
                });

                await Task.WhenAll(tasks);
            }
        }
    }
}