using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class InvitedUserEventHandler : IHandleMessages<InvitedUserEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;

    public InvitedUserEventHandler(ILegacyTopicMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(InvitedUserEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(
            new UserInvitedMessage(
                message.PersonInvited,
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}