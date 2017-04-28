using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateNewPeriodEnd;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetAllEmployerAccounts;
using SFA.DAS.EAS.Application.Queries.Payments.GetCurrentPeriodEnd;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.PaymentUpdater.WebJob.Updater;
using SFA.DAS.Messaging;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.EAS.PaymentUpdater.WebJob.UnitTests.UpdaterTests
{
    public class WhenIUpdatePayments
    {
        private PaymentProcessor _paymentProcessor;
        private Mock<IPaymentsEventsApiClient> _paymentsClient;
        private Mock<IMediator> _mediator;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<ILogger> _logger;
        private PaymentsApiClientConfiguration _configuration;
        private const long ExpectedAccountId = 12345444;


        [SetUp]
        public void Arrange()
        {
            _paymentsClient = new Mock<IPaymentsEventsApiClient>();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd { Id = "123456" } });
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>())).ReturnsAsync(new GetAllEmployerAccountsResponse { Accounts = new List<Account> { new Account { Id = ExpectedAccountId } } });

            _logger = new Mock<ILogger>();

            _configuration = new PaymentsApiClientConfiguration();

            _messagePublisher = new Mock<IMessagePublisher>();
            _paymentProcessor = new PaymentProcessor(_paymentsClient.Object, _mediator.Object, _messagePublisher.Object, _logger.Object, _configuration);
        }

        [Test]
        public async Task ThenThePaymentsApiShouldBeCheckedForTheCurrentPeriodEnd()
        {
            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _paymentsClient.Verify(x=>x.GetPeriodEnds(),Times.Once);
        }

        [Test]
        public async Task ThenTheCurrentPeriodEndShouldBeCheckedAgainstCurrentPeriodEndsAndTheAccountsQueryCalledIfThereAreUpdates()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd { Id = "123" } });
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new[] {new PeriodEnd {Id="123"}, new PeriodEnd {Id="1234", Links = new PeriodEndLinks { PaymentsForPeriod = "" } } });

            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>()), Times.Once);
            _mediator.Verify(x=>x.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>()), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenTheCurrentPeriodEndShouldBeCheckedAgainstCurrentPeriodEndsAndTheAccountsQueryNotCalledIfThereAreNoUpdates()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd { Id = "1234" } });
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new[] { new PeriodEnd { Id = "123" }, new PeriodEnd { Id = "1234" } });

            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>()), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>()), Times.Never);
            
        }

        [Test]
        public async Task ThenIfThereAreNoCurrentlyStoredPeriodsThenAllPeriodsAreRan()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse {  });
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new[] { new PeriodEnd { Id = "123", Links = new PeriodEndLinks { PaymentsForPeriod = "" } }, new PeriodEnd { Id = "1234", Links = new PeriodEndLinks { PaymentsForPeriod = "" } } });

            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()),Times.Exactly(2));
        }

        [Test]
        public async Task ThenTheAccountMessagesShouldBeAddedToTheQueueIfThePeriodEndHasChanged()
        {
            //Arrange
            var existingPeriodEnd = "123";
            var expectedPaymentsForPeriodUrl = "afd";
            var expectedPeriodEnd = "1234";
            
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd { Id = existingPeriodEnd } });
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new[] { new PeriodEnd { Id = existingPeriodEnd }, new PeriodEnd { Id = expectedPeriodEnd,Links = new PeriodEndLinks {PaymentsForPeriod = expectedPaymentsForPeriodUrl },ReferenceData = new ReferenceDataDetails {AccountDataValidAt = new DateTime(2016,01,01), CommitmentDataValidAt = new DateTime(2016, 01, 01) } } });

            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _messagePublisher.Verify(x=>x.PublishAsync(It.Is<PaymentProcessorQueueMessage>(
                                            c=>c.AccountId.Equals(ExpectedAccountId) 
                                            && c.PeriodEndId.Equals(expectedPeriodEnd)
                                            && c.AccountPaymentUrl.Equals($"{expectedPaymentsForPeriodUrl}&employeraccountid={ExpectedAccountId}")
                                            )), Times.Once());
        }

        [Test]
        public async Task ThenTheNewPeriodEndIsAddedToTheDatabase()
        {
            //Arrange
            var expectedAccountId = 12345;
            var expectedPaymentsForPeriodUrl = "afd";
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>())).ReturnsAsync(new GetAllEmployerAccountsResponse { Accounts = new List<Account> { new Account { Id = expectedAccountId } } });
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd { Id = "123" } });
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new[] { new PeriodEnd { Id = "123" }, new PeriodEnd { Id = "1234", Links = new PeriodEndLinks { PaymentsForPeriod = expectedPaymentsForPeriodUrl } } });

            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny< CreateNewPeriodEndCommand>()),Times.Once);
        }

        [Test]
        public async Task ThenWhenThereAreNoPeriodsTheAccountsAreNotRetrieved()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd {} });
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new PeriodEnd[] {} );

            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>()), Times.Never);
        }

        [Test]
        public async Task ThenWhenThereIsNoCommitmentDateValidAtOrAccountDataValidAtThenNoMessagesAreAddedToTheQueue()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd { } });
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>())).ReturnsAsync(new GetAllEmployerAccountsResponse { Accounts = new List<Account> { new Account { Id = 87544 } } });
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new[] { new PeriodEnd { Id = "1234", Links = new PeriodEndLinks { PaymentsForPeriod = "dd" },ReferenceData = {}} });

            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()), Times.Once);
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<PaymentProcessorQueueMessage>()), Times.Never);
        }

        [Test]
        public async Task ThenWhenThereIsNoCommitmentDateValidAtThenNoMessagesAreAddedToTheQueue()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd { } });
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>())).ReturnsAsync(new GetAllEmployerAccountsResponse { Accounts = new List<Account> { new Account { Id = 87544 } } });
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new[] { new PeriodEnd { Id = "1234", Links = new PeriodEndLinks { PaymentsForPeriod = "dd" }, ReferenceData = new ReferenceDataDetails {AccountDataValidAt= new DateTime(2016,01,01)} } });

            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()), Times.Once);
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<PaymentProcessorQueueMessage>()), Times.Never);
        }

        [Test]
        public async Task ThenWhenThereIsNoAccountDateValidAtThenNoMessagesAreAddedToTheQueue()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd { } });
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>())).ReturnsAsync(new GetAllEmployerAccountsResponse { Accounts = new List<Account> { new Account { Id = 87544 } } });
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new[] { new PeriodEnd { Id = "1234", Links = new PeriodEndLinks { PaymentsForPeriod = "dd" }, ReferenceData = new ReferenceDataDetails { CommitmentDataValidAt = new DateTime(2016, 01, 01) } } });

            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()), Times.Once);
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<PaymentProcessorQueueMessage>()), Times.Never);
        }

        [Test]
        public async Task ThenNoPaymentsAreProcessedWhenTheConfigIsSet()
        {
            //Arrange
            _configuration.PaymentsDisabled = true;
            var expectedAccountId = 12345;
            var expectedPaymentsForPeriodUrl = "afd";
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>())).ReturnsAsync(new GetAllEmployerAccountsResponse { Accounts = new List<Account> { new Account { Id = expectedAccountId } } });
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd { Id = "123" } });
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new[] { new PeriodEnd { Id = "123" }, new PeriodEnd { Id = "1234", Links = new PeriodEndLinks { PaymentsForPeriod = expectedPaymentsForPeriodUrl } } });

            //Act
            await _paymentProcessor.RunUpdate();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<CreateNewPeriodEndCommand>()), Times.Never);
            _logger.Verify(x=>x.Info("Payment processing disabled"));
        }
    }
}
