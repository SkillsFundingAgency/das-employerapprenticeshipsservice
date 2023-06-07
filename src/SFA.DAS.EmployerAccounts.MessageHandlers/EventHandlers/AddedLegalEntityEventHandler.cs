using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class AddedLegalEntityEventHandler : IHandleMessages<AddedLegalEntityEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;
    private readonly ILogger<AddedLegalEntityEventHandler> _logger;

    public AddedLegalEntityEventHandler(ILegacyTopicMessagePublisher messagePublisher, ILogger<AddedLegalEntityEventHandler> logger)
    {
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task Handle(AddedLegalEntityEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation($"Starting {nameof(AddedLegalEntityEventHandler)} handler for accountId: '{message.AccountId}'.");

        await _messagePublisher.PublishAsync(
            new LegalEntityAddedMessage(
                message.AccountId,
                message.AgreementId,
                message.OrganisationName,
                message.LegalEntityId,
                message.UserName,
                message.UserRef.ToString()));

        _logger.LogInformation($"Completed {nameof(AddedLegalEntityEventHandler)} handler.");
    }
}