using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Queries.FindAccountProviderPayments;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountTransactionOrchestratorTests
{
    internal class WhenIGetCoursePayments
    {
        private const string HashedAccountId = "123ABC";
        private const long AccountId = 1234;
        private const string ExternalUser = "Test user";
        private const long ExpectedUkPrn = 46789465;
        private readonly DateTime _fromDate = DateTime.Now.AddDays(-20);
        private readonly DateTime _toDate = DateTime.Now.AddDays(-20);
        private Mock<ICurrentDateTime> _currentTime;
        private Mock<IHashingService> _hashingService;

        private Mock<IMediator> _mediator;
        private EmployerAccountTransactionsOrchestrator _orchestrator;
        private FindAccountProviderPaymentsResponse _response;

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

            _mediator.Setup(AssertExpressionValidation()).ReturnsAsync(_response);

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(AccountId);

            _orchestrator =
                new EmployerAccountTransactionsOrchestrator(_mediator.Object, _currentTime.Object, Mock.Of<ILog>());
        }


        [Test]
        public async Task ThenIfNoTransactionsAreFoundANotFoundStatusIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<FindAccountProviderPaymentsQuery>()))
                .ThrowsAsync(new NotFoundException(string.Empty));

            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, ExpectedUkPrn, _fromDate,
                _toDate,
                ExternalUser);

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, result.Status);
        }

        [Test]
        public async Task ThenIfUserIsNotAuthorisedAUnauthorisedStatusIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<FindAccountProviderPaymentsQuery>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, ExpectedUkPrn, _fromDate,
                _toDate,
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
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, ExpectedUkPrn, _fromDate,
                _toDate,
                ExternalUser);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        }

        private Expression<Func<IMediator, Task<FindAccountProviderPaymentsResponse>>> AssertExpressionValidation()
        {
            return x => x.SendAsync(It.Is<FindAccountProviderPaymentsQuery>(c => c.ExternalUserId.Equals(ExternalUser)
                                                                                 && c.FromDate.Equals(_fromDate)
                                                                                 && c.ToDate.Equals(_toDate)
                                                                                 && c.HashedAccountId.Equals(
                                                                                     HashedAccountId)
                                                                                 && c.UkPrn.Equals(ExpectedUkPrn)));
        }
    }
}