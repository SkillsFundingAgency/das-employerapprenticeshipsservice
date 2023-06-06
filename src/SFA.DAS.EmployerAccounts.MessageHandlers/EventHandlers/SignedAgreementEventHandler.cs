using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class SignedAgreementEventHandler : IHandleMessages<SignedAgreementEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;
    private readonly ILogger<SignedAgreementEventHandler> _logger;

    public SignedAgreementEventHandler(ILegacyTopicMessagePublisher messagePublisher, ILogger<SignedAgreementEventHandler> logger)
    {
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task Handle(SignedAgreementEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation($"Starting {nameof(SignedAgreementEventHandler)} handler.");
        
        await _messagePublisher.PublishAsync(new AgreementSignedMessage(
            message.AccountId,
            message.AgreementId,
            message.OrganisationName,
            message.LegalEntityId,
            message.CohortCreated,
            message.UserName,
            message.UserRef.ToString()));
        
        _logger.LogInformation($"Completed {nameof(SignedAgreementEventHandler)} handler.");
    }
}