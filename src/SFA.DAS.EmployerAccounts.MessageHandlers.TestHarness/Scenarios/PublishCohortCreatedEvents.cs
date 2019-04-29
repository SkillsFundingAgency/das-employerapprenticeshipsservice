using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Cohort;

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
            var newEvent = new CohortCreated();

            await _messageSession.Publish(newEvent);

            // TODO: assert the document is updated
        }
    }
}
