using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.CreateNewPeriodEnd;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Queries.GetAllEmployerAccounts;
using SFA.DAS.EmployerFinance.Queries.GetCurrentPeriodEnd;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.CommandHandlers
{
    [TestFixture]
    public class GivenAnImportPaymentsCommand
    {
        private ImportPaymentsCommandHandler _sut;
        private Mock<IPaymentsEventsApiClient> _paymentsEventsApiClientMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<ILog> _loggerMock;
        private Mock<IMessageHandlerContext> _messageHandlerContextMock;
        private PaymentsApiClientConfiguration _configuration;
        private IFixture Fixture = new Fixture();
        private ImportPaymentsCommand _importPaymentsCommand;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _importPaymentsCommand = new ImportPaymentsCommand();
            Fixture.Customize<Account>(a => a.Without(x => x.AccountLegalEntities));
        }

        [SetUp]
        public void SetUp()
        {
            _messageHandlerContextMock = new Mock<IMessageHandlerContext>();
            _paymentsEventsApiClientMock = new Mock<IPaymentsEventsApiClient>();
            _mediatorMock = new Mock<IMediator>();
            _mediatorMock.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse());
            _loggerMock = new Mock<ILog>();
            _configuration = new PaymentsApiClientConfiguration { PaymentsDisabled = false };

            _sut = new ImportPaymentsCommandHandler(_paymentsEventsApiClientMock.Object, _mediatorMock.Object, _loggerMock.Object, _configuration);
        }

        [Test]
        [Category("UnitTest")]
        public async Task WhenPaymentsAreDisableThenDoNothing()
        {
            // Arrange
            _configuration.PaymentsDisabled = true;

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContextMock.Object);

            // Assert
            _paymentsEventsApiClientMock.Verify(x => x.GetPeriodEnds(), Times.Never);
        }

        [Test]
        [Category("UnitTest")]
        public async Task WhenPaymentsAreEnabledThenGetPaymentPeriods()
        {
            // Arrange

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContextMock.Object);

            // Assert
            _paymentsEventsApiClientMock.Verify(x => x.GetPeriodEnds(), Times.Once);
        }

        [Test]
        [Category("UnitTest")]
        public async Task WhenThereAreNoPaymentPeriodEndsThenDoNotProcessAnyPeriodEnds()
        {
            // Arrange

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContextMock.Object);

            // Assert
            _mediatorMock.Verify(x => x.SendAsync(
                It.IsAny<ICancellableAsyncRequest<CreateNewPeriodEndCommand>>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("UnitTest")]
        [TestCase(1, Description = "Single new period end to process")]
        [TestCase(5, Description = "Multiple new period ends to process")]
        public async Task WhenThereAreNewPeriodsThenCreateThem(int amountOfNewPeriodEnds)
        {
            // Arrange
            _paymentsEventsApiClientMock.Setup(mock => mock.GetPeriodEnds()).ReturnsAsync(GetPeriodEnds(amountOfNewPeriodEnds));

            _mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>()))
                .ReturnsAsync(new GetAllEmployerAccountsResponse
                {
                    Accounts = new List<Account> { Fixture.Create<Account>() }
                });

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContextMock.Object);

            // Assert
            _mediatorMock.Verify(
                x => x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()),
                Times.Exactly(amountOfNewPeriodEnds));
        }

        [Test]
        [Category("UnitTest")]
        [TestCase(1, Description = "Single existing period end")]
        [TestCase(5, Description = "Multiple existing period ends")]
        public async Task WhenAllCurrentPeriodsExistThenDoNotProcessAnyPeriods(int amountOfNewPeriodEnds)
        {
            // Arrange
            var apiPeriodEnds = GetPeriodEnds(amountOfNewPeriodEnds).OrderBy(pe => pe.Id).ToArray();
            _paymentsEventsApiClientMock.Setup(mock => mock.GetPeriodEnds()).ReturnsAsync(apiPeriodEnds);
            _mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>()))
                .ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new Models.Payments.PeriodEnd { PeriodEndId = apiPeriodEnds.Last().Id } });

            _mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>()))
                .ReturnsAsync(new GetAllEmployerAccountsResponse
                {
                    Accounts = new List<Account> { Fixture.Create<Account>() }
                });

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContextMock.Object);

            // Assert
            _mediatorMock.Verify(
                x => x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()),
                Times.Never);
        }

        [Test]
        [Category("UnitTest")]
        [TestCase(1, Description = "Single new period end")]
        [TestCase(5, Description = "Multiple new period ends")]
        public async Task WhenNewerPeriodEndsExistThenProcessTheNewPeriods(int amountOfNewPeriodEnds)
        {
            // Arrange
            var apiPeriodEnds = GetPeriodEnds(amountOfNewPeriodEnds+1).OrderBy(pe => pe.Id).ToArray();
            _paymentsEventsApiClientMock.Setup(mock => mock.GetPeriodEnds()).ReturnsAsync(apiPeriodEnds);
            _mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>()))
                .ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new Models.Payments.PeriodEnd { PeriodEndId = apiPeriodEnds.First().Id } });

            _mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>()))
                .ReturnsAsync(new GetAllEmployerAccountsResponse
                {
                    Accounts = new List<Account> { Fixture.Create<Account>() }
                });

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContextMock.Object);

            // Assert
            _mediatorMock.Verify(
                x => x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()),
                Times.Exactly(amountOfNewPeriodEnds));
        }

        private PeriodEnd[] GetPeriodEnds(int amountOfNewPeriodEnds)
        {
            return Fixture.CreateMany<PeriodEnd>(amountOfNewPeriodEnds).ToArray();
        }
    }
}
