using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.TempEvents;

namespace SFA.DAS.EAS.Portal.Jobs.TestHarness.Scenarios
{
    public class PublishReserveFundingAddedEvent
    {
        private readonly IMessageSession _messageSession;

        public PublishReserveFundingAddedEvent(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            const long accountId = 1337L;

            await _messageSession.Publish(new ReserveFundingAddedEvent
            {
                AccountId = accountId,
                AccountLegalEntityId = 420L,
                LegalEntityName = "Fishy Fingers Ltd",
                CourseId = 1,
                CourseName = "Fish Monger, Level 3 (Standard)",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2021, 1, 1),
                Created = DateTime.UtcNow
            });
        }
    }
}