using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
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
        [Ignore("In progress")]
        public Task Execute_WhenAccountDoesNotContainReservation_ThenAccountDocumentIsSavedWithNewReservation()
        {
            return TestAsync(f => f.Execute(), f => f.VerifyAccountDocumentSavedWithReservation());
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
        public const long AccountId = 456;

        public AddReservationCommandTestsFixture()
        {
            Fixture = new Fixture();

            AccountDocumentService = new Mock<IAccountDocumentService>();
            
            Logger = new Mock<ILogger<AddReservationCommand>>();

            AddReservationCommand = new AddReservationCommand(AccountDocumentService.Object, Logger.Object);
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
            
            if (AccountDocument == null)
            {
                expectedAccount = new Account
                {
                    Id = ExpectedReservationCreatedEvent.AccountId
                };
            }
            else
            {
                expectedAccount = AccountDocument.Account;
            }
            
            return document?.Account != null && document.Account.IsEqual(expectedAccount);
        }
    }
}