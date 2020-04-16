using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerFinance.Messages.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    //public class AccountLevyStatusEventHandler : IHandleMessages<AccountLevyStatusEvent>
    //{
    //    private readonly IMediator _mediatr;

    //    public AccountLevyStatusEventHandler(IMediator mediatr)
    //    {
    //        _mediatr = mediatr;
    //    }

    //    public async Task Handle(AccountLevyStatusEvent message, IMessageHandlerContext context)
    //    {
    //        await _mediatr.SendAsync(new AccountLevyStatusCommand
    //            {
    //                AccountId = message.AccountId,
    //                ApprenticeshipEmployerType = message.ApprenticeshipEmployerType
    //            });
    //    }
    //}
}