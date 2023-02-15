using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class HealthCheckEventHandler : IHandleMessages<HealthCheckEvent>
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public HealthCheckEventHandler(Lazy<EmployerAccountsDbContext> db)
    {
        _db = db;
    }

    public async Task Handle(HealthCheckEvent message, IMessageHandlerContext context)
    {
        var healthCheck = await _db.Value.HealthChecks.SingleAsync(h => h.Id == message.Id);

        healthCheck.ReceiveEvent(message);
    }
}