using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Testing;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using SFA.DAS.EAS.Portal.Application.EventHandlers.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.Reservations
{
    [TestFixture, Parallelizable]
    public class ReservationCreatedHandlerTests : FluentTest<ReservationCreatedHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyAccountDocumentSavedWithReservation());
        }

        [Test]
        public Task Handle_WhenAccountDoesNotContainOrganisation_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(f.Event.AccountId), f => f.Handle(),
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

    public class ReservationCreatedHandlerTestsFixture : EventHandlerTestsFixture<ReservationCreatedEvent, ReservationCreatedHandler>
    {
        public Mock<IAccountDocumentService> MockAccountDocumentService { get; set; }

        public AccountDocument AccountDocument { get; set; }
        public AccountDocument OriginalAccountDocument { get; set; }

        public const long AccountId = 456L;
        public const long AccountLegalEntityId = 789L;
        public Guid ReservationId = Guid.NewGuid();

        public ReservationCreatedHandlerTestsFixture()
        {
            MockAccountDocumentService = new Mock<IAccountDocumentService>();
            MockAccountDocumentService.Setup(m => m.GetOrCreate(AccountId, CancellationToken)).ReturnsAsync(new AccountDocument(AccountId));
            
            Handler = new ReservationCreatedHandler(MockAccountDocumentService.Object);

            Event.Id = ReservationId;
            Event.AccountId = AccountId;
            Event.AccountLegalEntityId = AccountLegalEntityId;
        }

        public override Task Handle()
        {
            OriginalAccountDocument = AccountDocument.Clone();

            return base.Handle();
        }

        public ReservationCreatedHandlerTestsFixture ArrangeEmptyAccountDocument(long accountId)
        {
            AccountDocument = JsonConvert.DeserializeObject<AccountDocument>($"{{\"Account\": {{\"Id\": {accountId} }}}}");

            MockAccountDocumentService.Setup(s => s.GetOrCreate(accountId, CancellationToken)).ReturnsAsync(AccountDocument);

            return this;
        }

        public ReservationCreatedHandlerTestsFixture ArrangeAccountDocumentContainsOrganisation()
        {
            var organisation = SetUpAccountDocumentWithOrganisation(Event.AccountId, AccountLegalEntityId);
            organisation.Reservations = new List<Reservation>();

            MockAccountDocumentService.Setup(s => s.GetOrCreate(Event.AccountId, CancellationToken)).ReturnsAsync(AccountDocument);

            return this;
        }

        public ReservationCreatedHandlerTestsFixture ArrangeAccountDocumentContainsReservation()
        {
            var organisation = SetUpAccountDocumentWithOrganisation(Event.AccountId, AccountLegalEntityId);

            organisation.Reservations.RandomElement().Id = ReservationId;

            MockAccountDocumentService.Setup(s => s.GetOrCreate(Event.AccountId, CancellationToken)).ReturnsAsync(AccountDocument);

            return this;
        }

        public void VerifyAccountDocumentSavedWithReservation()
        {
            MockAccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => AccountIsAsExpected(d)), CancellationToken), Times.Once);
        }

        public Organisation SetUpAccountDocumentWithOrganisation(long accountId, long accountLegalEntityId)
        {
            AccountDocument = Fixture.Create<AccountDocument>();

            AccountDocument.Account.Id = accountId;

            AccountDocument.Deleted = null;
            AccountDocument.Account.Deleted = null;

            var organisation = AccountDocument.Account.Organisations.RandomElement();
            organisation.AccountLegalEntityId = accountLegalEntityId;

            return organisation;
        }

        public bool AccountIsAsExpected(AccountDocument document)
        {
            var expectedAccount = GetExpectedAccount(OriginalEvent.AccountId);
            var expectedReservation = GetExpectedReservation(
                GetExpectedOrganisation(expectedAccount, AccountLegalEntityId, OriginalEvent.AccountLegalEntityName));

            expectedReservation.Id = ReservationId;
            expectedReservation.CourseCode = OriginalEvent.CourseId;
            expectedReservation.CourseName = OriginalEvent.CourseName;
            expectedReservation.StartDate = OriginalEvent.StartDate;
            expectedReservation.EndDate = OriginalEvent.EndDate;

            return AccountIsAsExpected(expectedAccount, document);
        }

        public bool AccountIsAsExpected(Account expectedAccount, AccountDocument savedAccountDocument)
        {
            if (savedAccountDocument?.Account == null)
                return false;

            var (accountIsAsExpected, differences) = savedAccountDocument.Account.IsEqual(expectedAccount);
            if (!accountIsAsExpected)
            {
                TestContext.WriteLine($"Saved account is not as expected: {differences}");
            }

            return accountIsAsExpected;
        }

        public Organisation GetExpectedOrganisation(Account expectedAccount, long accountLegalEntityId, string accountLegalEntityName)
        {
            if (OriginalAccountDocument != null && OriginalAccountDocument.Account.Organisations.Any())
                return expectedAccount.Organisations.Single(o => o.AccountLegalEntityId == accountLegalEntityId);

            var expectedOrganisation = new Organisation
            {
                AccountLegalEntityId = accountLegalEntityId,
                Name = accountLegalEntityName
            };
            expectedAccount.Organisations.Add(expectedOrganisation);

            return expectedOrganisation;
        }

        public Account GetExpectedAccount(long accountId)
        {
            if (OriginalAccountDocument == null)
            {
                return new Account
                {
                    Id = accountId,
                };
            }
            return OriginalAccountDocument.Account;
        }

        public Reservation GetExpectedReservation(Organisation expectedOrganisation)
        {
            Reservation expectedReservation;
            if (OriginalAccountDocument == null
                || !OriginalAccountDocument.Account.Organisations.Any())
            {
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
