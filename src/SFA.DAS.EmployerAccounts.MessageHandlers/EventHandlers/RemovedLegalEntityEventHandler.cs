using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class RemovedLegalEntityEventHandler : IHandleMessages<RemovedLegalEntityEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;

    public RemovedLegalEntityEventHandler(ILegacyTopicMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(RemovedLegalEntityEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(
            new LegalEntityRemovedMessage(
                message.AccountId,
                message.AgreementId,
                message.AgreementSigned,
                message.LegalEntityId,
                message.OrganisationName,
                message.UserName,
                message.UserRef.ToString()));
    }
}