using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class DeletedPayeSchemeEventHandler : IHandleMessages<DeletedPayeSchemeEvent>
{
    private readonly IEventPublisher _messagePublisher;

    public DeletedPayeSchemeEventHandler(IEventPublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public Task Handle(DeletedPayeSchemeEvent message, IMessageHandlerContext context)
    {
        return _messagePublisher.Publish(
            new PayeSchemeDeletedMessage(
                message.PayeRef,
                message.OrganisationName,
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}