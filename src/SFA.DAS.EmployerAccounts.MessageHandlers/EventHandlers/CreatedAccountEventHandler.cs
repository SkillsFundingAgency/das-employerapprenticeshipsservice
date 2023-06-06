using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class CreatedAccountEventHandler : IHandleMessages<CreatedAccountEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;

    public CreatedAccountEventHandler(ILegacyTopicMessagePublisher messagePublisher)
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