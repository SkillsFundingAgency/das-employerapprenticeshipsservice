using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerFinance;

public class RefreshEmployerLevyDataCompletedEventHandler : IHandleMessages<RefreshEmployerLevyDataCompletedEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;
    private readonly IMediator _mediator;

    public RefreshEmployerLevyDataCompletedEventHandler(ILegacyTopicMessagePublisher messagePublisher, IMediator mediator)
    {
        _messagePublisher = messagePublisher;
        _mediator = mediator;
    }

    public async Task Handle(RefreshEmployerLevyDataCompletedEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(new RefreshEmployerLevyDataCompletedMessage(
            message.AccountId,
            message.LevyImported,
            message.PeriodMonth,
            message.PeriodYear,
            message.Created,
            string.Empty,
            string.Empty));

        await _mediator.Send(new AccountLevyStatusCommand
        {
            AccountId = message.AccountId,
            ApprenticeshipEmployerType = message.LevyTransactionValue == decimal.Zero ? ApprenticeshipEmployerType.NonLevy : ApprenticeshipEmployerType.Levy
        });
    }
}