using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class DeletedPayeSchemeEventHandler : IHandleMessages<DeletedPayeSchemeEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;

    public DeletedPayeSchemeEventHandler(ILegacyTopicMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public Task Handle(DeletedPayeSchemeEvent message, IMessageHandlerContext context)
    {
        return _messagePublisher.PublishAsync(
            new PayeSchemeDeletedMessage(
                message.PayeRef,
                message.OrganisationName,
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}