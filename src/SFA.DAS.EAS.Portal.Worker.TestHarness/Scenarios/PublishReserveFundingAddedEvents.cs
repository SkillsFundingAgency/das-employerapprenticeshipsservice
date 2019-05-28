using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Reservations.Messages;

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
            const long accountId = 1L;
            const long accountLegalEntityId1 = 1L;
            const string legalEntityName1 = "Fishy Fingers Ltd";
            const long accountLegalEntityId2 = 8008135L;
            const string legalEntityName2 = "Ann Chovy's Fish Emporium Ltd";

            await _messageSession.Publish(new ReservationCreatedEvent(
                new Guid("11111111-ECFD-4D28-BB76-2DA9ECE6AD2C"),
                accountLegalEntityId1, legalEntityName1, "3",
                new DateTime(2020, 1, 1),
                "Fish Monger, Level 3 (Standard)", new DateTime(2021, 1, 1),
                DateTime.UtcNow, accountId));

            Console.WriteLine("Published ReservationCreatedEvent. Id: 11111111-ECFD-4D28-BB76-2DA9ECE6AD2C");
            
            // another reservation, same account, same legal entity
            await _messageSession.Publish(new ReservationCreatedEvent(
                new Guid("22222222-ECFD-4D28-BB76-2DA9ECE6AD2C"),
                accountLegalEntityId1, legalEntityName1, "4",
                new DateTime(2020, 2, 1),
                "Fish Monger, Level 4 (Standard)", new DateTime(2021, 2, 1),
                DateTime.UtcNow, accountId));

            Console.WriteLine("Published ReservationCreatedEvent. Id: 22222222-ECFD-4D28-BB76-2DA9ECE6AD2C");

            // another reservation, same account, different legal entity
            await _messageSession.Publish(new ReservationCreatedEvent(
                new Guid("33333333-ECFD-4D28-BB76-2DA9ECE6AD2C"),
                accountLegalEntityId2, legalEntityName2, "2",
                new DateTime(2020, 2, 1),
                "Fish Monger, Level 2 (Standard)", new DateTime(2021, 2, 1),
                DateTime.UtcNow, accountId));
            
            Console.WriteLine("Published ReservationCreatedEvent. Id: 33333333-ECFD-4D28-BB76-2DA9ECE6AD2C");
        }
    }
}