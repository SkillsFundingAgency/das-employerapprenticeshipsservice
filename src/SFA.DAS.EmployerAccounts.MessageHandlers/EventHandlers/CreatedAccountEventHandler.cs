using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class CreatedAccountEventHandler : IHandleMessages<CreatedAccountEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;
    private readonly ILogger<CreatedAccountEventHandler> _logger;

    public CreatedAccountEventHandler(ILegacyTopicMessagePublisher messagePublisher, ILogger<CreatedAccountEventHandler> logger)
    {
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation($"Starting {nameof(CreatedAccountEventHandler)} handler for accountId: '{message.AccountId}'.");

        await _messagePublisher.PublishAsync(
            new AccountCreatedMessage(
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));

        _logger.LogInformation($"Completed {nameof(CreatedAccountEventHandler)} handler.");
    }
}