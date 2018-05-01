using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.FindAccountProviderPayments;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Exceptions;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountTransactionOrchestratorTests
{
    class WhenIGetPaymentDetails
    {
        private const string HashedAccountId = "123ABC";
        private readonly Guid ExternalUser =Guid.NewGuid();
        private const long AccountId = 1234;
        private readonly DateTime _fromDate = DateTime.Now.AddDays(-20);
        private readonly DateTime _toDate = DateTime.Now.AddDays(-20);

        private Mock<IMediator> _mediator;
        private EmployerAccountTransactionsOrchestrator _orchestrator;
        private FindAccountProviderPaymentsResponse _response;
        private Mock<ICurrentDateTime> _currentTime;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _currentTime = new Mock<ICurrentDateTime>();
            _hashingService = new Mock<IHashingService>();

            _response = new FindAccountProviderPaymentsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = 100,
                Transactions = new List<PaymentTransactionLine>
                {
                    new PaymentTransactionLine {Amount = 100}
                }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<FindAccountProviderPaymentsQuery>()))
                .ReturnsAsync(_response);

            _orchestrator = new EmployerAccountTransactionsOrchestrator(_mediator.Object, _currentTime.Object, Mock.Of<ILog>());
        }

        [Test]
        public async Task ThenARequestShouldBeMadeForPaymentDetails()
        {
            //Arrange
            const long ukprn = 10;

            //Act
            await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, ukprn, _fromDate, _toDate,
                ExternalUser);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<FindAccountProviderPaymentsQuery>(
                q => q.HashedAccountId.Equals(HashedAccountId) &&
                q.FromDate.Equals(_fromDate) &&
                q.ToDate.Equals(_toDate) &&
                q.ExternalUserId.Equals(ExternalUser))));
        }

        [Test]
        public async Task ThenPaymentsDetailsShouldBeReturned()
        {
            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, 10, _fromDate, _toDate,
                ExternalUser);

            //Assert
            Assert.IsNotEmpty(result.Data.SubTransactions);
            Assert.AreEqual(_response.Transactions[0], result.Data.SubTransactions[0]);
            Assert.AreEqual(HttpStatusCode.OK, result.Status);
        }

        [Test]
        public async Task ThenIfNoTransactionsAreFoundANullIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<FindAccountProviderPaymentsQuery>()))
                     .ReturnsAsync(null);

            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, 10, _fromDate, _toDate,
                ExternalUser);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task ThenIfUserIsNotAuthorisedAUnauthorisedStatusIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<FindAccountProviderPaymentsQuery>()))
                     .ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, 10, _fromDate, _toDate,
                ExternalUser);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.Status);
        }

        [Test]
        public async Task ThenIfRequestIsNotValidABadRequestStatusIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<FindAccountProviderPaymentsQuery>()))
                     .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, 10, _fromDate, _toDate,
                ExternalUser);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        }

        [Test]
        public void ThenIShouldGetPaymentsGroupedByCourse()
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
