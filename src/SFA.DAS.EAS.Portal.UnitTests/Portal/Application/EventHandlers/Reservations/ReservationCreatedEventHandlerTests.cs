using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.EventHandlers.Reservations;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.Reservations
{
    [TestFixture, Parallelizable]
    public class ReservationCreatedEventHandlerTests : FluentTest<ReservationCreatedEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyAccountDocumentSavedWithReservation());
        }

        [Test]
        public Task Handle_WhenAccountDoesNotContainOrganisation_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(f.Message.AccountId),f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithReservation());
        }

        [Test]
        public Task Handle_WhenAccountDoesContainOrganisationButNotReservation_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsOrganisation(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithReservation());
        }
        
        [Test]
        public Task Handle_WhenAccountDoesContainOrganisationAndReservation_ThenExceptionIsThrown()
        {
            return TestExceptionAsync(f => f.ArrangeAccountDocumentContainsReservation(), f => f.Handle(),
                (f, r) => r.Should().Throw<Exception>().Where(ex => ex.Message.StartsWith("Received ReservationCreatedEvent with")));
        }
    }

    public class ReservationCreatedEventHandlerTestsFixture
    {
        public EventHandlerTestsFixture<ReservationCreatedEvent, ReservationCreatedEventHandler> EventHandlerTestsFixture { get; set; }

        public const long AccountLegalEntityId = 456L;
        public Guid ReservationId = Guid.NewGuid();

        public ReservationCreatedEvent Message
        {
            get => EventHandlerTestsFixture.Message;
            set => EventHandlerTestsFixture.Message = value;
        }

        public ReservationCreatedEventHandlerTestsFixture()
        {
            EventHandlerTestsFixture = new EventHandlerTestsFixture<ReservationCreatedEvent, ReservationCreatedEventHandler>();
            
            //todo: let test use fixture generated?
            Message.Id = ReservationId;
            Message.AccountLegalEntityId = AccountLegalEntityId;
        }

        public ReservationCreatedEventHandlerTestsFixture ArrangeEmptyAccountDocument(long accountId)
        {
            EventHandlerTestsFixture.ArrangeEmptyAccountDocument(accountId);

            return this;
        }
        
        //todo: move to base
        public ReservationCreatedEventHandlerTestsFixture ArrangeAccountDocumentContainsOrganisation()
        {
            var organisation = EventHandlerTestsFixture.SetUpAccountDocumentWithOrganisation(Message.AccountId, AccountLegalEntityId);
            organisation.Reservations = new List<Reservation>();
            
            EventHandlerTestsFixture.AccountDocumentService.Setup(
                s => s.Get(Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventHandlerTestsFixture.AccountDocument);
            
            return this;
        }
        
        public ReservationCreatedEventHandlerTestsFixture ArrangeAccountDocumentContainsReservation()
        {
            var organisation = EventHandlerTestsFixture.SetUpAccountDocumentWithOrganisation(Message.AccountId, AccountLegalEntityId);

            organisation.Reservations.RandomElement().Id = ReservationId;
            
            EventHandlerTestsFixture.AccountDocumentService.Setup(s => s.Get(
                Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(EventHandlerTestsFixture.AccountDocument);
            
            return this;
        }
        
        public Task Handle()
        {
            return EventHandlerTestsFixture.Handle();
        }
        
        public void VerifyAccountDocumentSavedWithReservation()
        {
            EventHandlerTestsFixture.AccountDocumentService.Verify(s => s.Save(
                It.Is<AccountDocument>(d => AccountIsAsExpected(d)),It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        private bool AccountIsAsExpected(AccountDocument document)
        {
            var expectedAccount = EventHandlerTestsFixture.GetExpectedAccount(EventHandlerTestsFixture.OriginalMessage.AccountId);
            var expectedReservation = GetExpectedReservation(
                EventHandlerTestsFixture.GetExpectedOrganisation(
                    expectedAccount, AccountLegalEntityId, EventHandlerTestsFixture.OriginalMessage.AccountLegalEntityName));

            expectedReservation.Id = ReservationId;
            expectedReservation.CourseCode = EventHandlerTestsFixture.OriginalMessage.CourseId;
            expectedReservation.CourseName = EventHandlerTestsFixture.OriginalMessage.CourseName;
            expectedReservation.StartDate = EventHandlerTestsFixture.OriginalMessage.StartDate;
            expectedReservation.EndDate = EventHandlerTestsFixture.OriginalMessage.EndDate;

            return EventHandlerTestsFixture.AccountIsAsExpected(expectedAccount, document);
        }

        private Reservation GetExpectedReservation(Organisation expectedOrganisation)
        {
            Reservation expectedReservation;
            if (EventHandlerTestsFixture.OriginalAccountDocument == null
                || !EventHandlerTestsFixture.OriginalAccountDocument.Account.Organisations.Any())
            {
                //todo: AddNewReservation()?
                expectedReservation = new Reservation();
                expectedOrganisation.Reservations.Add(expectedReservation);
                return expectedReservation;
            }
            
            expectedReservation = expectedOrganisation.Reservations.SingleOrDefault(r => r.Id == ReservationId);
            if (expectedReservation == null)
            {
                expectedReservation = new Reservation();
                expectedOrganisation.Reservations.Add(expectedReservation);
            }
            return expectedReservation;
        }
    }
}