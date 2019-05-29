using SFA.DAS.EmployerAccounts.Messages.Events;
using System;
using NServiceBus;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Worker.TestHarness.Scenarios
{
    class PublishAddedPayeSchemeEvents
    {
        private readonly IMessageSession _messageSession;

        public PublishAddedPayeSchemeEvents(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            await _messageSession.Publish(new AddedPayeSchemeEvent
            {
                AccountId = 1,
                UserName = "Bob",
                UserRef = new Guid(),
                PayeRef = "BestPayeScheme",
                Created = DateTime.Now
            });

            Console.WriteLine("Published AddedPayeSchemeEvent for AccountId 1");
        }
    }
}
