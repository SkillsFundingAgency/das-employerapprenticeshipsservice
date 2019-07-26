using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.ProviderRelationships.Messages.Events;

namespace SFA.DAS.EAS.Portal.Worker.TestHarness.Scenarios
{
    public class PublishAddedAccountProviderEvent
    {
        private readonly IMessageSession _messageSession;

        public PublishAddedAccountProviderEvent(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            const long accountId = 27446L;
            const long ukprn1 = 10005077L;

            await _messageSession.Publish(new AddedAccountProviderEvent(
                1, accountId, ukprn1,
                new Guid("1D2A5DA7-6133-44F8-BF69-BD16AFA645DE"),
                DateTime.UtcNow));
            
            Console.WriteLine($"Published {nameof(AddedAccountProviderEvent)}.");
        }
    }
}