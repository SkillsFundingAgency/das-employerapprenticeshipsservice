using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.CreateNewPeriodEnd;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Queries.GetAllEmployerAccounts;
using SFA.DAS.EmployerFinance.Queries.GetPeriodEnds;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;
using DbPeriodEnd = SFA.DAS.EmployerFinance.Models.Payments.PeriodEnd;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.CommandHandlers
{
    [TestFixture]
    public class GivenAnImportPaymentsCommand
    {
        private ImportPaymentsCommandHandler _sut;
        private Mock<IPaymentsEventsApiClient> _paymentsEventsApiClientMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<ILog> _loggerMock;
        private TestableMessageHandlerContext _messageHandlerContext;
        private PaymentsEventsApiClientLocalConfiguration _configuration;
        private IFixture Fixture = new Fixture();
        private ImportPaymentsCommand _importPaymentsCommand;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _importPaymentsCommand = new ImportPaymentsCommand();
        }

        [SetUp]
        public void SetUp()
        {
            _messageHandlerContext = new TestableMessageHandlerContext();
            _paymentsEventsApiClientMock = new Mock<IPaymentsEventsApiClient>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILog>();

            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetPeriodEndsRequest>()))
                .ReturnsAsync(new GetPeriodEndsResponse { CurrentPeriodEnds = new List<DbPeriodEnd>() });

            Fixture.Customize<Account>(x => x.Without(s => s.AccountLegalEntities));
            Fixture.Customize(new AutoMoqCustomization());

            _mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>()))
                .ReturnsAsync(new GetAllEmployerAccountsResponse { Accounts = new List<Account> { Fixture.Create<Account>() } });

            _configuration = new PaymentsEventsApiClientLocalConfiguration { PaymentsDisabled = false };

            _sut = new ImportPaymentsCommandHandler(_paymentsEventsApiClientMock.Object, _mediatorMock.Object, _loggerMock.Object, _configuration);
        }

        [Test]
        [Category("UnitTest")]
        public async Task WhenPaymentsAreDisableThenDoNothing()
        {
            // Arrange
            _configuration.PaymentsDisabled = true;

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContext);

            // Assert
            _paymentsEventsApiClientMock.Verify(x => x.GetPeriodEnds(), Times.Never);
        }

        [Test]
        [Category("UnitTest")]
        public async Task WhenPaymentsAreEnabledThenGetPaymentPeriods()
        {
            // Arrange

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContext);

            // Assert
            _paymentsEventsApiClientMock.Verify(x => x.GetPeriodEnds(), Times.Once);
        }

        [Test]
        [Category("UnitTest")]
        public async Task WhenThereAreNoPaymentPeriodEndsThenDoNotProcessAnyPeriodEnds()
        {
            // Arrange

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContext);

            // Assert
            _mediatorMock.Verify(x => x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()), Times.Never);
        }

        [Test]
        [Category("UnitTest")]
        [TestCase(1, Description = "Single new period end to process")]
        [TestCase(5, Description = "Multiple new period ends to process")]
        public async Task WhenThereAreNoCurrentPeriodsAndThereAreNewPeriodsThenCreateThem(int amountOfNewPeriodEnds)
        {
            // Arrange
            var apiPeriodEnds = GetApiPeriodEnds(amountOfNewPeriodEnds);
            _paymentsEventsApiClientMock.Setup(mock => mock.GetPeriodEnds()).ReturnsAsync(apiPeriodEnds);

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContext);

            // Assert
            VerifyCreateNewPeriodSentForEachMissingPeriod(apiPeriodEnds, new List<DbPeriodEnd>(), amountOfNewPeriodEnds);
        }

        [Test]
        [Category("UnitTest")]
        [TestCase(1, Description = "Single existing period end")]
        [TestCase(5, Description = "Multiple existing period ends")]
        public async Task WhenAllCurrentPeriodsExistThenDoNotCreateAnyPeriodEnds(int amountOfNewPeriodEnds)
        {
            // Arrange
            var apiPeriodEnds = GetApiPeriodEnds(amountOfNewPeriodEnds);
            var existingPeriodEnds = GetDbMatchedPeriodEnds(apiPeriodEnds);
            _paymentsEventsApiClientMock.Setup(mock => mock.GetPeriodEnds()).ReturnsAsync(apiPeriodEnds);
            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetPeriodEndsRequest>()))
                .ReturnsAsync(new GetPeriodEndsResponse { CurrentPeriodEnds = existingPeriodEnds });

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContext);

            // Assert
            _mediatorMock.Verify(x => x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()), Times.Never);
        }

        [Test]
        [Category("UnitTest")]
        [TestCase(1, Description = "Single new subsqeunt period end")]
        [TestCase(5, Description = "Multiple new subsequent period ends")]
        public async Task WhenCurrentPeriodsExistAndThereAreNewSubsequentPeriodsThenCreateMissingPeriods(int amountOfNewPeriodEnds)
        {
            // Arrange
            var apiPeriodEnds = GetApiPeriodEnds(amountOfNewPeriodEnds + 1);
            var existingPeriodEnds = GetDbMatchedPeriodEnds(apiPeriodEnds).Take(1).ToList();
            _paymentsEventsApiClientMock.Setup(mock => mock.GetPeriodEnds()).ReturnsAsync(apiPeriodEnds);

            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetPeriodEndsRequest>()))
                .ReturnsAsync(new GetPeriodEndsResponse { CurrentPeriodEnds = existingPeriodEnds });

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContext);

            // Assert
            VerifyCreateNewPeriodSentForEachMissingPeriod(apiPeriodEnds, existingPeriodEnds, amountOfNewPeriodEnds);
        }

        [Test]
        [Category("UnitTest")]
        public async Task WhenCurrentPeriodsExistAndThereAreNewPeriodsPreviousCreateMissingPeriods()
        {
            // Arrange
            var apiPeriodEnds = GetApiPeriodEnds(5);
            var existingPeriodEnds = GetDbMatchedPeriodEnds(apiPeriodEnds).Skip(2).Take(2).ToList();
            _paymentsEventsApiClientMock.Setup(mock => mock.GetPeriodEnds()).ReturnsAsync(apiPeriodEnds);

            _mediatorMock.Setup(mock => mock.SendAsync(It.IsAny<GetPeriodEndsRequest>()))
                .ReturnsAsync(new GetPeriodEndsResponse { CurrentPeriodEnds = existingPeriodEnds });

            // Act
            await _sut.Handle(_importPaymentsCommand, _messageHandlerContext);

            // Assert
            VerifyCreateNewPeriodSentForEachMissingPeriod(apiPeriodEnds, existingPeriodEnds, 3);
        }

        private void VerifyCreateNewPeriodSentForEachMissingPeriod(PeriodEnd[] apiPeriodEnds, List<DbPeriodEnd> existingPeriodEnds, int expectedNumberOfNewPeriodEnds)
        {
            var expectedNewPeriodIds = GetExpectedPeriodEndIds(apiPeriodEnds, existingPeriodEnds);
            foreach (var expectedId in expectedNewPeriodIds)
            {
                _mediatorMock.Verify(x => x.SendAsync(It.Is<CreateNewPeriodEndCommand>(command => command.NewPeriodEnd.PeriodEndId == expectedId)), Times.Once);
            }
        }

        private PeriodEnd[] GetApiPeriodEnds(int amountOfNewPeriodEnds)
        {
            return Fixture.CreateMany<PeriodEnd>(amountOfNewPeriodEnds).OrderBy(pe => pe.Id).ToArray();
        }

        private List<DbPeriodEnd> GetDbMatchedPeriodEnds(PeriodEnd[] apiPeriodEnds)
        {
            return apiPeriodEnds.Select(pe => new DbPeriodEnd { PeriodEndId = pe.Id }).ToList();
        }

        private IEnumerable<string> GetExpectedPeriodEndIds(PeriodEnd[] apiPeriodEnds, List<DbPeriodEnd> existingPeriodEnds)
        {
            return apiPeriodEnds.Where(pe => !existingPeriodEnds.Any(cpe => cpe.PeriodEndId == pe.Id)).Select(pe => pe.Id);
        }
    }
}
