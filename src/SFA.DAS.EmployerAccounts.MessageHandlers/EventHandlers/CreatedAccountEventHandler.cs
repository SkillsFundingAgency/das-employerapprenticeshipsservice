using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class CreatedAccountEventHandler : IHandleMessages<CreatedAccountEvent>
{
    private readonly IEventPublisher _messagePublisher;

    public CreatedAccountEventHandler(IEventPublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.Publish(
            new AccountCreatedMessage(
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}