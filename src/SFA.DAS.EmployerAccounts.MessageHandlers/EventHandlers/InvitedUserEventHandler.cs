using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class InvitedUserEventHandler : IHandleMessages<InvitedUserEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public InvitedUserEventHandler(IMessagePublisher messagePublisher)
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