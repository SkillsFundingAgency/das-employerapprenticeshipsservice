using System;
using System.Data.Entity;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Events;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class HealthCheckEventHandler : IHandleMessages<HealthCheckEvent>
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public HealthCheckEventHandler(Lazy<EmployerFinanceDbContext> db)
        {
            _db = db;
        }

        public async Task Handle(HealthCheckEvent message, IMessageHandlerContext context)
        {
            var healthCheck = await _db.Value.HealthChecks.SingleAsync(h => h.Id == message.Id);

            healthCheck.ReceiveEvent(message);
        }
    }
}