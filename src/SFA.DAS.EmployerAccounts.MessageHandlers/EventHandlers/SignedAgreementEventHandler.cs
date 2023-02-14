using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class SignedAgreementEventHandler : IHandleMessages<SignedAgreementEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public SignedAgreementEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(SignedAgreementEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(new AgreementSignedMessage(
            message.AccountId,
            message.AgreementId,
            message.OrganisationName,
            message.LegalEntityId,
            message.CohortCreated,
            message.UserName,
            message.UserRef.ToString()));
    }
}