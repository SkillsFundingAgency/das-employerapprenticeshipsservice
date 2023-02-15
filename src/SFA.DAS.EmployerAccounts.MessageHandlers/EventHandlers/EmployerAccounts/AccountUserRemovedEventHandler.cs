using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerAccounts;

public class AccountUserRemovedEventHandler : IHandleMessages<AccountUserRemovedEvent>
{
    private readonly IMediator _mediator;

    public AccountUserRemovedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Handle(AccountUserRemovedEvent message, IMessageHandlerContext context)
    {
        await _mediator.Send(new RemoveAccountUserCommand(message.AccountId, message.UserRef, context.MessageId, message.Created));
    }
}