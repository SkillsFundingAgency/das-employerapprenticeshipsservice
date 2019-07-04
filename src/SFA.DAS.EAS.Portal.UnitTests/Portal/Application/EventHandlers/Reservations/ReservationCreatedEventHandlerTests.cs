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
            return TestAsync(f => f.ArrangeAccountDoesNotExist(f.Message.AccountId), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithReservation());
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

    public class ReservationCreatedEventHandlerTestsFixture : EventHandlerBaseTestFixture<ReservationCreatedEvent, ReservationCreatedEventHandler>
    {
        public AccountDocHelper AccountDocHelper { get; set; }

        public const long AccountLegalEntityId = 456L;
        public Guid ReservationId = Guid.NewGuid();

        public ReservationCreatedEventHandlerTestsFixture()
        {
            AccountDocHelper = new AccountDocHelper();
            
            //todo: let test use fixture generated?
            Message.Id = ReservationId;
            Message.AccountLegalEntityId = AccountLegalEntityId;

            Handler = new ReservationCreatedEventHandler(
                AccountDocHelper.AccountDocumentService.Object, 
                Logger.Object);
        }

        public ReservationCreatedEventHandlerTestsFixture ArrangeAccountDoesNotExist(long accountId)
        {
            AccountDocHelper.ArrangeAccountDoesNotExist(accountId);

            return this;
        }
        
        public ReservationCreatedEventHandlerTestsFixture ArrangeEmptyAccountDocument(long accountId)
        {
            AccountDocHelper.ArrangeEmptyAccountDocument(accountId);

            return this;
        }
        
        //todo: move to base
        public ReservationCreatedEventHandlerTestsFixture ArrangeAccountDocumentContainsOrganisation()
        {
            var organisation = AccountDocHelper.SetUpAccountDocumentWithOrganisation(Message.AccountId, AccountLegalEntityId);
            organisation.Reservations = new List<Reservation>();
            
            AccountDocHelper.AccountDocumentService.Setup(
                s => s.GetOrCreate(Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(AccountDocHelper.AccountDocument);
            
            return this;
        }
        
        public ReservationCreatedEventHandlerTestsFixture ArrangeAccountDocumentContainsReservation()
        {
            var organisation = AccountDocHelper.SetUpAccountDocumentWithOrganisation(Message.AccountId, AccountLegalEntityId);

            organisation.Reservations.RandomElement().Id = ReservationId;
            
            AccountDocHelper.AccountDocumentService.Setup(s => s.GetOrCreate(
                Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(AccountDocHelper.AccountDocument);
            
            return this;
        }
        
        public void VerifyAccountDocumentSavedWithReservation()
        {
            AccountDocHelper.AccountDocumentService.Verify(s => s.Save(
                It.Is<AccountDocument>(d => AccountIsAsExpected(d)),It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        private bool AccountIsAsExpected(AccountDocument document)
        {
            var expectedAccount = AccountDocHelper.GetExpectedAccount(OriginalMessage.AccountId);
            var expectedReservation = GetExpectedReservation(
                AccountDocHelper.GetExpectedOrganisation(
                    expectedAccount, AccountLegalEntityId, OriginalMessage.AccountLegalEntityName));

            expectedReservation.Id = ReservationId;
            expectedReservation.CourseCode = OriginalMessage.CourseId;
            expectedReservation.CourseName = OriginalMessage.CourseName;
            expectedReservation.StartDate = OriginalMessage.StartDate;
            expectedReservation.EndDate = OriginalMessage.EndDate;

            return AccountDocHelper.AccountIsAsExpected(expectedAccount, document);
        }

        public override Task Handle()
        {
            AccountDocHelper.OriginalAccountDocument = AccountDocHelper.AccountDocument.Clone();
            return base.Handle();
        }

        private Reservation GetExpectedReservation(Organisation expectedOrganisation)
        {
            Reservation expectedReservation;
            if (AccountDocHelper.OriginalAccountDocument == null
                || !AccountDocHelper.OriginalAccountDocument.Account.Organisations.Any())
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