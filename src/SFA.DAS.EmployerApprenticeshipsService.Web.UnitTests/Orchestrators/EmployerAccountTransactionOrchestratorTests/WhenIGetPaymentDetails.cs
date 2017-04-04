using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.FindEmployerAccountPaymentTransactions;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountTransactionOrchestratorTests
{
    class WhenIGetPaymentDetails
    {
        private const string HashedAccountId = "123ABC";
        private const string ExternalUser = "Test user";
        private readonly DateTime _fromDate = DateTime.Now.AddDays(-20);
        private readonly DateTime _toDate = DateTime.Now.AddDays(-20);

        private Mock<IMediator> _mediator;
        private EmployerAccountTransactionsOrchestrator _orchestrator;
        private FindEmployerAccountPaymentTransactionsResponse _response;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _response = new FindEmployerAccountPaymentTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = 100,
                Transactions = new List<PaymentTransactionLine>
                {
                    new PaymentTransactionLine {Amount = 100}
                }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<FindEmployerAccountPaymentTransactionsQuery>()))
                .ReturnsAsync(_response);

            _orchestrator = new EmployerAccountTransactionsOrchestrator(_mediator.Object);
        }

        [Test]
        public async Task ThenARequestShouldBeMadeForPaymentDetails()
        {
            //Act
            await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, _fromDate, _toDate,
                ExternalUser);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<FindEmployerAccountPaymentTransactionsQuery>(
                q => q.HashedAccountId.Equals(HashedAccountId) &&
                q.FromDate.Equals(_fromDate) &&
                q.ToDate.Equals(_toDate) &&
                q.ExternalUserId.Equals(ExternalUser))));
        }

        [Test]
        public async Task ThenPaymentsDetailsShouldBeReturned()
        {
            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, _fromDate, _toDate,
                ExternalUser);

            //Assert
            Assert.IsNotEmpty(result.Data.SubTransactions);
            Assert.AreEqual(_response.Transactions[0], result.Data.SubTransactions[0]);
            Assert.AreEqual(HttpStatusCode.OK, result.Status);
        }

        [Test]
        public async Task ThenIfNoTransactionsAreFoundANotFoundStatusIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<FindEmployerAccountPaymentTransactionsQuery>()))
                     .ThrowsAsync(new NotFoundException(string.Empty));

            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, _fromDate, _toDate,
                ExternalUser);

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, result.Status);
        }

        [Test]
        public async Task ThenIfUserIsNotAuthorisedAUnauthorisedStatusIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<FindEmployerAccountPaymentTransactionsQuery>()))
                     .ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, _fromDate, _toDate,
                ExternalUser);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.Status);
        }

        [Test]
        public async Task ThenIfRequestIsNotValidABadRequestStatusIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<FindEmployerAccountPaymentTransactionsQuery>()))
                     .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, _fromDate, _toDate,
                ExternalUser);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        }

        [Test]
        public async Task ThenIShouldGetPaymentsGroupedByCourse()
        {
            //Arrange
            _response.Transactions = new List<PaymentTransactionLine>
            {
                new PaymentTransactionLine
                {
                    
                }
            };

            //Act


            //Assert
        }
    }
}
