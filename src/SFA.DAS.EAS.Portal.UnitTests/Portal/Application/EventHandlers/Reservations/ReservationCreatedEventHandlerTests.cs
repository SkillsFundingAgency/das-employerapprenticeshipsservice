using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        public Task Handle_WhenAccountDoesContainOrganisationAndTheReservationAlreadyexists_ThenAccountDocumentIsNotSavedWiththeDuplicateReservation()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsReservation(), f => f.Handle(),
                f => f.VerifyAccountDocumentNotSavedWithReservation());
        }

        [Test]
        public Task Handle_WhenAccountDoesContainOrganisationAndTheReservationAlreadyexists_ThenTheDuplicateReservationEventIsLogged()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsReservation(), f => f.Handle(),
                f => f.VerifyDuplicateReservationEventIsLogged());
        }
    }

    public class ReservationCreatedEventHandlerTestsFixture
    {
        public AccountEventHandlerTestHelper<ReservationCreatedEvent, ReservationCreatedEventHandler> Helper { get; set; }

        public const long AccountLegalEntityId = 456L;
        public Guid ReservationId = Guid.NewGuid();

        public ReservationCreatedEvent Message
        {
            get => Helper.Message;
            set => Helper.Message = value;
        }

        public ReservationCreatedEventHandlerTestsFixture()
        {
            Helper = new AccountEventHandlerTestHelper<ReservationCreatedEvent, ReservationCreatedEventHandler>();
            
            //todo: let test use fixture generated?
            Message.Id = ReservationId;
            Message.AccountLegalEntityId = AccountLegalEntityId;
        }

        public ReservationCreatedEventHandlerTestsFixture ArrangeAccountDoesNotExist(long accountId)
        {
            Helper.ArrangeAccountDoesNotExist(accountId);

            return this;
        }
        
        public ReservationCreatedEventHandlerTestsFixture ArrangeEmptyAccountDocument(long accountId)
        {
            Helper.ArrangeEmptyAccountDocument(accountId);

            return this;
        }
        
        //todo: move to base
        public ReservationCreatedEventHandlerTestsFixture ArrangeAccountDocumentContainsOrganisation()
        {
            var organisation = Helper.SetUpAccountDocumentWithOrganisation(Message.AccountId, AccountLegalEntityId);
            organisation.Reservations = new List<Reservation>();
            
            Helper.AccountDocumentService.Setup(
                s => s.GetOrCreate(Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Helper.AccountDocument);
            
            return this;
        }
        
        public ReservationCreatedEventHandlerTestsFixture ArrangeAccountDocumentContainsReservation()
        {
            var organisation = Helper.SetUpAccountDocumentWithOrganisation(Message.AccountId, AccountLegalEntityId);

            organisation.Reservations.RandomElement().Id = ReservationId;
            
            Helper.AccountDocumentService.Setup(s => s.GetOrCreate(
                Message.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Helper.AccountDocument);
            
            return this;
        }
        
        public Task Handle()
        {
            return Helper.Handle();
        }
        
        public void VerifyAccountDocumentSavedWithReservation()
        {
            Helper.AccountDocumentService.Verify(s => s.Save(
                It.Is<AccountDocument>(d => AccountIsAsExpected(d)),It.IsAny<CancellationToken>()),
                Times.Once);
        }

        public void VerifyAccountDocumentNotSavedWithReservation()
        {
            Helper.AccountDocumentService.Verify(s => s.Save(
                It.Is<AccountDocument>(d => AccountIsAsExpected(d)), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        public void VerifyDuplicateReservationEventIsLogged()
        {
            //just check a warning is logged
            Helper.Logger.Verify(s => s.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }

        private bool AccountIsAsExpected(AccountDocument document)
        {
            var expectedAccount = Helper.GetExpectedAccount(Helper.OriginalMessage.AccountId);
            var expectedReservation = GetExpectedReservation(
                Helper.GetExpectedOrganisation(
                    expectedAccount, AccountLegalEntityId, Helper.OriginalMessage.AccountLegalEntityName));

            expectedReservation.Id = ReservationId;
            expectedReservation.CourseCode = Helper.OriginalMessage.CourseId;
            expectedReservation.CourseName = Helper.OriginalMessage.CourseName;
            expectedReservation.StartDate = Helper.OriginalMessage.StartDate;
            expectedReservation.EndDate = Helper.OriginalMessage.EndDate;

            return Helper.AccountIsAsExpected(expectedAccount, document);
        }

        private Reservation GetExpectedReservation(Organisation expectedOrganisation)
        {
            Reservation expectedReservation;
            if (Helper.OriginalAccountDocument == null
                || !Helper.OriginalAccountDocument.Account.Organisations.Any())
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