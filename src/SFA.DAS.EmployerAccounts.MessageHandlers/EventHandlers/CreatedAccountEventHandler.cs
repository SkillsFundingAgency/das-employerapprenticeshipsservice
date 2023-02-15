using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class CreatedAccountEventHandler : IHandleMessages<CreatedAccountEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public CreatedAccountEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(
            new AccountCreatedMessage(
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}