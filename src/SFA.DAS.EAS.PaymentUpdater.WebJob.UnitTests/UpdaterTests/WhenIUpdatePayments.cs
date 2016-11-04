﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.EAS.Application.Commands.CreateNewPeriodEnd;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetAllEmployerAccounts;
using SFA.DAS.EAS.Application.Queries.Payments.GetCurrentPeriodEnd;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.PaymentUpdater.WebJob.Updater;
using SFA.DAS.Messaging;
using SFA.DAS.Payments.Events.Api.Client;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.PaymentUpdater.WebJob.UnitTests.UpdaterTests
{
    public class WhenIUpdatePayments
    {
        private PaymentProcessor _paymentProcessor;
        private Mock<IPaymentsEventsApiClient> _paymentsClient;
        private Mock<IMediator> _mediator;
        private Mock<IMessagePublisher> _messagePublisher;
        private const long ExpectedAccountId = 12345444;


        [SetUp]
        public void Arrange()
        {
            _paymentsClient = new Mock<IPaymentsEventsApiClient>();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCurrentPeriodEndRequest>())).ReturnsAsync(new GetPeriodEndResponse { CurrentPeriodEnd = new PeriodEnd { Id = "123456" } });
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAllEmployerAccountsRequest>())).ReturnsAsync(new GetAllEmployerAccountsResponse { Accounts = new List<Account> { new Account { Id = ExpectedAccountId } } });

            _messagePublisher = new Mock<IMessagePublisher>();
            _paymentProcessor = new PaymentProcessor(_paymentsClient.Object, _mediator.Object, _messagePublisher.Object);
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
            _paymentsClient.Setup(x => x.GetPeriodEnds()).ReturnsAsync(new[] { new PeriodEnd { Id = existingPeriodEnd }, new PeriodEnd { Id = expectedPeriodEnd,Links = new PeriodEndLinks {PaymentsForPeriod = expectedPaymentsForPeriodUrl } } });

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
    }
}
