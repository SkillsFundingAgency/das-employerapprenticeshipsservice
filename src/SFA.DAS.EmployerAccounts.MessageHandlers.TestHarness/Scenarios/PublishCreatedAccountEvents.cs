using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Scenarios;

public class PublishCreatedAccountEvents
{
    private readonly IMessageSession _messageSession;

    public PublishCreatedAccountEvents(IMessageSession messageSession)
    {
        _messageSession = messageSession;
    }

    public Task Run()
    {
        var newEvent = new CreatedAccountEvent();
        _messageSession.Publish(newEvent);

        return Task.CompletedTask;
    }
}