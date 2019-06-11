using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;

namespace SFA.DAS.EAS.Portal.Worker.TestHarness.Scenarios
{
    public class PublishCohortApprovedByEmployerEvents
    {
        private readonly IMessageSession _messageSession;

        public PublishCohortApprovedByEmployerEvents(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            const long accountId = 3L;

            await _messageSession.Publish(new CohortApprovedByEmployer
            {
                AccountId = accountId,
                ProviderId = 456,
                CommitmentId = 789
            });

            Console.WriteLine($"Published {nameof(CohortApprovedByEmployer)}. ProviderId: 456, CommitmentId = 789");
            
        }
    }
}