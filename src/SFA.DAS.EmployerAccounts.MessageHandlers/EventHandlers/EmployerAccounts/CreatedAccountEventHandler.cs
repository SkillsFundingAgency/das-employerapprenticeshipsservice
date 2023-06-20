using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerAccounts;

public class CreatedAccountEventHandler : IHandleMessages<CreatedAccountEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreatedAccountEventHandler> _logger;

    public CreatedAccountEventHandler(IMediator mediator, ILogger<CreatedAccountEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    public async Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation($"{nameof(CreatedAccountEvent)} received for Account: {message.HashedId}");

        await _mediator.Send(new CreateAccountUserCommand(message.AccountId, message.UserRef, UserRole.Owner, context.MessageId, message.Created));
    }
}