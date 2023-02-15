using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Commitments.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Scenarios;

public class PublishCohortCreatedEvents
{
    private readonly IMessageSession _messageSession;

    public PublishCohortCreatedEvents(IMessageSession messageSession)
    {
        _messageSession = messageSession;
    }

    public Task Run()
    {
        var newEvent = new CohortApprovalRequestedByProvider();

        _messageSession.Publish(newEvent);

        // TODO: assert the document is updated
        return Task.CompletedTask;
    }
}