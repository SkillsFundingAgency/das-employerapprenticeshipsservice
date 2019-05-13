using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Portal.Events.Reservations;

namespace SFA.DAS.EAS.Portal.Worker.TestHarness.Scenarios
{
    public class PublishReserveFundingAddedEvents
    {
        private readonly IMessageSession _messageSession;

        public PublishReserveFundingAddedEvents(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            const long accountId = 1337L;
            const long accountLegalEntityId1 = 420L;
            const string legalEntityName1 = "Fishy Fingers Ltd";
            const long accountLegalEntityId2 = 8008135L;
            const string legalEntityName2 = "Ann Chovy's Fish Emporium Ltd";

            await _messageSession.Publish(new ReservationCreatedEvent
            {
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId1,
                AccountLegalEntityName = legalEntityName1,
                Id = new Guid("11111111-ECFD-4D28-BB76-2DA9ECE6AD2C"),
                CourseId = "3",
                CourseName = "Fish Monger, Level 3 (Standard)",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2021, 1, 1),
                CreatedDate = DateTime.UtcNow
            });

            // another reservation, same account, same legal entity
            await _messageSession.Publish(new ReservationCreatedEvent
            {
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId1,
                AccountLegalEntityName = legalEntityName1,
                Id = new Guid("22222222-ECFD-4D28-BB76-2DA9ECE6AD2C"),
                CourseId = "4",
                CourseName = "Fish Monger, Level 4 (Standard)",
                StartDate = new DateTime(2020, 2, 1),
                EndDate = new DateTime(2021, 2, 1),
                CreatedDate = DateTime.UtcNow
            });
            
            // another reservation, same account, different legal entity
            await _messageSession.Publish(new ReservationCreatedEvent
            {
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId2,
                AccountLegalEntityName = legalEntityName2,
                Id = new Guid("33333333-ECFD-4D28-BB76-2DA9ECE6AD2C"),
                CourseId = "2",
                CourseName = "Fish Monger, Level 2 (Standard)",
                StartDate = new DateTime(2020, 2, 1),
                EndDate = new DateTime(2021, 2, 1),
                CreatedDate = DateTime.UtcNow
            });
        }
    }
}