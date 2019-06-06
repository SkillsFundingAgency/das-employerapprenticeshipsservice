using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
        
        [Test]
        public Task Handle_WhenAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyAccountDocumentSavedWithReservation());
        }

        [Test]
        public Task Handle_WhenAccountDoesNotContainOrganisation_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(),f => f.Handle(), f => f.VerifyAccountDocumentSavedWithReservation());
        }

        [Test]
        public Task Handle_WhenAccountDoesContainOrganisationButNotReservation_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsOrganisation(), f => f.Handle(), f => f.VerifyAccountDocumentSavedWithReservation());
        }
        
        [Test]
        public Task Handle_WhenAccountDoesContainOrganisationAndReservation_ThenExceptionIsThrown()
        {
            return TestExceptionAsync(f => f.ArrangeAccountDocumentContainsReservation(), f => f.Handle(),
                (f, r) => r.Should().Throw<Exception>().Where(ex => ex.Message.StartsWith("Received ReservationCreatedEvent with")));
        }
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

        public ReservationCreatedEventHandlerTestsFixture ArrangeAccountDocumentContainsOrganisation()
        {
            var organisation = SetUpAccountDocumentWithOrganisation(AccountLegalEntityId);
            organisation.Reservations = new List<Reservation>();
            
            AccountDocumentService.Setup(s => s.Get(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
            return this;
        }
        
        public ReservationCreatedEventHandlerTestsFixture ArrangeAccountDocumentContainsReservation()
        {
            var organisation = SetUpAccountDocumentWithOrganisation(AccountLegalEntityId);

            organisation.Reservations.RandomElement().Id = ReservationId;
            
            AccountDocumentService.Setup(s => s.Get(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
            return this;
        }
        
        public ReservationCreatedEventHandlerTestsFixture VerifyAccountDocumentSavedWithReservation()
        {
            //todo: improve message on fail
            AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => AccountIsAsExpected(d)),It.IsAny<CancellationToken>()), Times.Once);
            
            return this;
        }
        
        private bool AccountIsAsExpected(AccountDocument document)
        {
            Account expectedAccount;
            Reservation expectedReservation;
            
            //todo: tidy up and move what can into base
            if (OriginalAccountDocument == null)
            {
                expectedAccount = new Account
                {
                    Id = OriginalMessage.AccountId,
                };

                var organisation = new Organisation
                {
                    AccountLegalEntityId = AccountLegalEntityId,
                    Name = OriginalMessage.AccountLegalEntityName
                };
                expectedAccount.Organisations.Add(organisation);

                expectedReservation = new Reservation();
                organisation.Reservations.Add(expectedReservation);
            }
            else if (!OriginalAccountDocument.Account.Organisations.Any())
            {
                expectedAccount = OriginalAccountDocument.Account;

                var organisation = new Organisation
                {
                    AccountLegalEntityId = AccountLegalEntityId,
                    Name = OriginalMessage.AccountLegalEntityName
                };
                expectedAccount.Organisations.Add(organisation);

                expectedReservation = new Reservation();
                organisation.Reservations.Add(expectedReservation);
            }
            else
            {
                expectedAccount = OriginalAccountDocument.Account;
                var expectedOrganisation = expectedAccount
                    .Organisations.Single(o => o.AccountLegalEntityId == AccountLegalEntityId);
                
                expectedReservation = expectedOrganisation
                    .Reservations.SingleOrDefault(r => r.Id == ReservationId);
                
                if (expectedReservation == null)
                {
                    expectedReservation = new Reservation();
                    expectedOrganisation.Reservations.Add(expectedReservation);
                }
            }

            expectedReservation.Id = ReservationId;
            expectedReservation.CourseCode = OriginalMessage.CourseId;
            expectedReservation.CourseName = OriginalMessage.CourseName;
            expectedReservation.StartDate = OriginalMessage.StartDate;
            expectedReservation.EndDate = OriginalMessage.EndDate;
            
            if (document?.Account == null)
                return false;
            
            var (accountIsAsExpected, differences) = document.Account.IsEqual(expectedAccount);
            if (!accountIsAsExpected)
            {
                TestContext.WriteLine($"Saved account is not as expected: {differences}");
            }
            
            return accountIsAsExpected;
        }
    }
}