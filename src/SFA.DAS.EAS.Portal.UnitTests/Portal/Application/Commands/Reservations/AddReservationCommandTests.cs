using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.Commands.Reservation;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Commands.Reservations
{
    [Parallelizable]
    [TestFixture]
    public class AddReservationCommandTests : FluentTest<AddReservationCommandTestsFixture>
    {
        [Test]
        public Task Execute_WhenAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.Execute(), f => f.VerifyAccountDocumentSavedWithReservation());
        }

        [Test]
        public Task Execute_WhenAccountDoesNotContainOrganisationOrReservation_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(),f => f.Execute(), f => f.VerifyAccountDocumentSavedWithReservation());
        }

        [Test]
        public Task Execute_WhenAccountDoesContainOrganisationButNotReservation_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsOrganisation(), f => f.Execute(), f => f.VerifyAccountDocumentSavedWithReservation());
        }
        
        [Test]
        public Task Execute_WhenAccountDoesContainOrganisationAndReservation_ThenExceptionIsThrown()
        {
            return TestExceptionAsync(f => f.ArrangeAccountDocumentContainsReservation(), f => f.Execute(),
                (f, r) => r.Should().Throw<Exception>().Where(ex => ex.Message.StartsWith("Received ReservationCreatedEvent with")));
        }
    }

    public class AddReservationCommandTestsFixture
    {
        public AddReservationCommand AddReservationCommand { get; set; }
        public Mock<IAccountDocumentService> AccountDocumentService { get; set; }
        public AccountDocument AccountDocument { get; set; }
        public Mock<ILogger<AddReservationCommand>> Logger { get; set; }
        public ReservationCreatedEvent ReservationCreatedEvent { get; set; }
        public ReservationCreatedEvent ExpectedReservationCreatedEvent { get; set; }
        public Fixture Fixture { get; set; }
        public const long AccountId = 456L;
        public const long AccountLegalEntityId = 789L;
        public Guid ReservationId = Guid.NewGuid();

        public AddReservationCommandTestsFixture()
        {
            Fixture = new Fixture();

            AccountDocumentService = new Mock<IAccountDocumentService>();
            
            Logger = new Mock<ILogger<AddReservationCommand>>();

            AddReservationCommand = new AddReservationCommand(AccountDocumentService.Object, Logger.Object);

            ReservationCreatedEvent = Fixture.Create<ReservationCreatedEvent>();
            ReservationCreatedEvent.Id = ReservationId;
            ReservationCreatedEvent.AccountId = AccountId;
            ReservationCreatedEvent.AccountLegalEntityId = AccountLegalEntityId;
        }
        
        public AddReservationCommandTestsFixture ArrangeEmptyAccountDocument()
        {
            AccountDocument = new AccountDocument(AccountId) {IsNew = false};

            AccountDocumentService.Setup(s => s.Get(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
            return this;
        }

        private Organisation SetUpAccountDocumentWithOrganisation()
        {
            AccountDocument = Fixture.Create<AccountDocument>();

            AccountDocument.Account.Id = AccountId;
            
            AccountDocument.Deleted = null;
            AccountDocument.Account.Deleted = null;
            
            //todo: helper for picking random
            var organisation = AccountDocument.Account.Organisations.Skip(new Random().Next(AccountDocument.Account.Organisations.Count))
                .First();
            organisation.AccountLegalEntityId = AccountLegalEntityId;

            return organisation;
        }
        
        public AddReservationCommandTestsFixture ArrangeAccountDocumentContainsOrganisation()
        {
            var organisation = SetUpAccountDocumentWithOrganisation();
            organisation.Reservations = new List<Reservation>();
            
            AccountDocumentService.Setup(s => s.Get(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
            return this;
        }
        
        public AddReservationCommandTestsFixture ArrangeAccountDocumentContainsReservation()
        {
            var organisation = SetUpAccountDocumentWithOrganisation();

            organisation.Reservations.Skip(new Random().Next(organisation.Reservations.Count)).First().Id = ReservationId;
            
            AccountDocumentService.Setup(s => s.Get(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
            return this;
        }
        
        public async Task Execute()
        {
            ExpectedReservationCreatedEvent = ReservationCreatedEvent.Clone();
            
            await AddReservationCommand.Execute(ReservationCreatedEvent, CancellationToken.None);
        }
        
        public AddReservationCommandTestsFixture VerifyAccountDocumentSavedWithReservation()
        {
            AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => AccountIsAsExpected(d)),It.IsAny<CancellationToken>()), Times.Once);
            
            return this;
        }
        
        public bool AccountIsAsExpected(AccountDocument document)
        {
            Account expectedAccount;
            Reservation expectedReservation;
            
            if (AccountDocument == null)
            {
                expectedAccount = new Account
                {
                    Id = ExpectedReservationCreatedEvent.AccountId,
                };

                var organisation = new Organisation
                {
                    AccountLegalEntityId = AccountLegalEntityId,
                    Name = ExpectedReservationCreatedEvent.AccountLegalEntityName
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
            expectedReservation.CourseCode = ExpectedReservationCreatedEvent.CourseId;
            expectedReservation.CourseName = ExpectedReservationCreatedEvent.CourseName;
            expectedReservation.StartDate = ExpectedReservationCreatedEvent.StartDate;
            expectedReservation.EndDate = ExpectedReservationCreatedEvent.EndDate;
            
            return document?.Account != null && document.Account.IsEqual(expectedAccount);
        }
    }
}