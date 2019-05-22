using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;

namespace SFA.DAS.EAS.Portal.Worker.TestHarness.Scenarios
{
    public class PublishCohortApprovalRequestedByProviderEvents
    {
        private readonly IMessageSession _messageSession;

        public PublishCohortApprovalRequestedByProviderEvents(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            const long accountId = 3L;

            await _messageSession.Publish(new CohortApprovalRequestedByProvider
            {
                AccountId = accountId,
                ProviderId = 456,
                CommitmentId = 789
            });

            Console.WriteLine("Published CohortApprovalRequestedByProvider. ProviderId: 456, CommitmentId = 789");
            
        }
    }
}