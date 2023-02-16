using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class InvitedUserEventHandler : IHandleMessages<InvitedUserEvent>
{
    private readonly IEventPublisher _messagePublisher;

    public InvitedUserEventHandler(IEventPublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(InvitedUserEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.Publish(
            new UserInvitedMessage(
                message.PersonInvited,
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}