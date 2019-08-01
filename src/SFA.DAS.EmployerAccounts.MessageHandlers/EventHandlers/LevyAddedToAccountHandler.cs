using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerFinance.Messages.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class LevyAddedToAccountHandler : IHandleMessages<LevyAddedToAccount>
    {
        private readonly IMediator _mediatr;

        public LevyAddedToAccountHandler(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        public async Task Handle(LevyAddedToAccount message, IMessageHandlerContext context)
        {
            await 
                _mediatr
                    .SendAsync(
                        new UpdateAccountToLevy(
                            message.AccountId)
                    );
        }
    }
}