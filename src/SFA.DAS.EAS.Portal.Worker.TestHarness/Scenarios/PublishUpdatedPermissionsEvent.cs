using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.EAS.Portal.Worker.TestHarness.Scenarios
{
    public class PublishUpdatedPermissionsEvent
    {
        private readonly IMessageSession _messageSession;

        public PublishUpdatedPermissionsEvent(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            const long accountId = 1L;
            const long accountLegalEntityId1 = 1L;
            const long ukprn1 = 1;//todo

            await _messageSession.Publish(new UpdatedPermissionsEvent(
                accountId, accountLegalEntityId1,
                accountLegalEntityId1, 1, ukprn1,
                new Guid("1D2A5DA7-6133-44F8-BF69-BD16AFA645DE"),
                new HashSet<Operation>() {Operation.CreateCohort},
                DateTime.UtcNow));
            
            Console.WriteLine("Published UpdatedPermissionsEvent.");
        }
    }
}