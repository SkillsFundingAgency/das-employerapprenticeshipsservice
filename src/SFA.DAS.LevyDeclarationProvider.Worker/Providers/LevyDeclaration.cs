using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.Messaging;

namespace SFA.DAS.LevyDeclarationProvider.Worker.Providers
{
    public class LevyDeclaration : ILevyDeclaration
    {
        private readonly IPollingMessageReceiver _pollingMessageReceiver;

        public LevyDeclaration(IPollingMessageReceiver pollingMessageReceiver)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
        }

        public async Task Handle()
        {
            var message = await _pollingMessageReceiver.ReceiveAsAsync<QueueMessage>();

            
        }
    }
}
