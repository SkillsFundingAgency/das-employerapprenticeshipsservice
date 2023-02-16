using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class ChangedAccountNameEventHandler : IHandleMessages<ChangedAccountNameEvent>
{
    private readonly IEventPublisher _messagePublisher;

    public ChangedAccountNameEventHandler(IEventPublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(ChangedAccountNameEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.Publish(new AccountNameChangedMessage(
            message.PreviousName,
            message.CurrentName,
            message.AccountId,
            message.UserName,
            message.UserRef.ToString()));
    }
}