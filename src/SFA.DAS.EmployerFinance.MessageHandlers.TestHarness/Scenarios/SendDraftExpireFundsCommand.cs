using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.MessageHandlers.TestHarness.Scenarios
{
    public class SendDraftExpireFundsCommand
    {
        private readonly IMessageSession _messageSession;

        public SendDraftExpireFundsCommand(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            var newCommand = new DraftExpireFundsCommand { DateTo = new DateTime(2018, 5, 1) };
            await _messageSession.Send(newCommand);
        }
    }
}
