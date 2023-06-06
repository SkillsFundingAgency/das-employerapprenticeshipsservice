using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class AddedPayeSchemeEventHandler : IHandleMessages<AddedPayeSchemeEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;

    public AddedPayeSchemeEventHandler(ILegacyTopicMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(AddedPayeSchemeEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(
            new PayeSchemeAddedMessage(
                message.PayeRef,
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}