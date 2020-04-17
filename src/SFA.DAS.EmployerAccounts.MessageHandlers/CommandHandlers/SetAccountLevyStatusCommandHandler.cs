using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.CommandHandlers
{
    public class SetAccountLevyStatusCommandHandler : IHandleMessages<SetAccountLevyStatusCommand>
    {
        private readonly IMediator _mediatr;

        public SetAccountLevyStatusCommandHandler(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        public async Task Handle(SetAccountLevyStatusCommand message, IMessageHandlerContext context)
        {
            await _mediatr.SendAsync(new AccountLevyStatusCommand
            {
                AccountId = message.AccountId,
                ApprenticeshipEmployerType = message.ApprenticeshipEmployerType
            });
        }
    }
}