using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Scenarios
{
    public class PublishCreatedAccountEvents
    {
        private readonly IMessageSession _messageSession;

        public PublishCreatedAccountEvents(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            var newEvent = new CreatedAccountEvent();
            await _messageSession.Publish(newEvent);

            // TODO: assert the document is updated
        }
    }
}
