using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.Reservations;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Reservations
{
    [TestFixture]
    [Parallelizable]
    public class ReservationCreatedEventHandlerTests : FluentTest<ReservationCreatedEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingReservationCreatedEvent_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }
        
//        [Test]
//        public Task Handle_WhenAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewReservation()
//        {
//            return TestAsync(f => f.Handle(), f => f.VerifyAccountDocumentSavedWithReservation());
//        }

        [Test]
        public Task Handle_WhenAccountDoesNotContainOrganisationOrReservation_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(),f => f.Handle(), f => f.VerifyAccountDocumentSavedWithReservation());
        }

//        [Test]
//        public Task Handle_WhenAccountDoesContainOrganisationButNotReservation_ThenAccountDocumentIsSavedWithNewReservation()
//        {
//            return TestAsync(f => f.ArrangeAccountDocumentContainsOrganisation(), f => f.Handle(), f => f.VerifyAccountDocumentSavedWithReservation());
//        }
//        
//        [Test]
//        public Task Handle_WhenAccountDoesContainOrganisationAndReservation_ThenExceptionIsThrown()
//        {
//            return TestExceptionAsync(f => f.ArrangeAccountDocumentContainsReservation(), f => f.Handle(),
//                (f, r) => r.Should().Throw<Exception>().Where(ex => ex.Message.StartsWith("Received ReservationCreatedEvent with")));
//        }
    }

    public class ReservationCreatedEventHandlerTestsFixture : EventHandlerTestsFixture<
        ReservationCreatedEvent, ReservationCreatedEventHandler>
    {
        public const long AccountLegalEntityId = 789L;
        public Guid ReservationId = Guid.NewGuid();

        public ReservationCreatedEventHandlerTestsFixture()
        {
            //todo: let test use fixture generated?
            Message.Id = ReservationId;
            Message.AccountId = AccountId;
            Message.AccountLegalEntityId = AccountLegalEntityId;
        }

        public ReservationCreatedEventHandlerTestsFixture VerifyAccountDocumentSavedWithReservation()
        {
            AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => AccountIsAsExpected(d)),It.IsAny<CancellationToken>()), Times.Once);
            
            return this;
        }
        
        private bool AccountIsAsExpected(AccountDocument document)
        {
            Account expectedAccount;
            Reservation expectedReservation;
            
            if (AccountDocument == null)
            {
                expectedAccount = new Account
                {
                    Id = ExpectedMessage.AccountId,
                };

                var organisation = new Organisation
                {
                    AccountLegalEntityId = AccountLegalEntityId,
                    Name = ExpectedMessage.AccountLegalEntityName
                };
                expectedAccount.Organisations.Add(organisation);

                expectedReservation = new Reservation();
                organisation.Reservations.Add(expectedReservation);
            }
            else
            {
                expectedAccount = AccountDocument.Account;
                expectedReservation = expectedAccount
                    .Organisations.Single(o => o.AccountLegalEntityId == AccountLegalEntityId)
                    .Reservations.Single(r => r.Id == ReservationId);
            }

            expectedReservation.Id = ReservationId;
            expectedReservation.CourseCode = ExpectedMessage.CourseId;
            expectedReservation.CourseName = ExpectedMessage.CourseName;
            expectedReservation.StartDate = ExpectedMessage.StartDate;
            expectedReservation.EndDate = ExpectedMessage.EndDate;
            
            return document?.Account != null && document.Account.IsEqual(expectedAccount);
        }
    }
}