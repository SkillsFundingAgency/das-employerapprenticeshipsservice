using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Commitments.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Scenarios
{
    public class PublishCohortCreatedEvents
    {
        private readonly IMessageSession _messageSession;

        public PublishCohortCreatedEvents(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            var newEvent = new CohortApprovalRequestedByProvider();

            await _messageSession.Publish(newEvent);

            // TODO: assert the document is updated
        }
    }
}
