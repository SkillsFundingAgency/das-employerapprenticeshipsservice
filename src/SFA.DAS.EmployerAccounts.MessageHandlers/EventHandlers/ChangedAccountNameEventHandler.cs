using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class ChangedAccountNameEventHandler : IHandleMessages<ChangedAccountNameEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public ChangedAccountNameEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(ChangedAccountNameEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(new AccountNameChangedMessage(
            message.PreviousName,
            message.CurrentName,
            message.AccountId,
            message.UserName,
            message.UserRef.ToString()));
    }
}