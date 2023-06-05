using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class SignedAgreementEventHandler : IHandleMessages<SignedAgreementEvent>
{
    private readonly IEventPublisher _messagePublisher;
    private readonly ILogger<SignedAgreementEventHandler> _loger;

    public SignedAgreementEventHandler(IEventPublisher messagePublisher, ILogger<SignedAgreementEventHandler> loger)
    {
        _messagePublisher = messagePublisher;
        _loger = loger;
    }

    public async Task Handle(SignedAgreementEvent message, IMessageHandlerContext context)
    {
        _loger.LogDebug($"Starting {nameof(SignedAgreementEventHandler)} handler.");
        
        await _messagePublisher.Publish(new AgreementSignedMessage(
            message.AccountId,
            message.AgreementId,
            message.OrganisationName,
            message.LegalEntityId,
            message.CohortCreated,
            message.UserName,
            message.UserRef.ToString()));
        
        _loger.LogDebug($"Completed {nameof(SignedAgreementEventHandler)} handler.");
    }
}