using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class ChangedAccountNameEventHandler : IHandleMessages<ChangedAccountNameEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;

    public ChangedAccountNameEventHandler(ILegacyTopicMessagePublisher messagePublisher)
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