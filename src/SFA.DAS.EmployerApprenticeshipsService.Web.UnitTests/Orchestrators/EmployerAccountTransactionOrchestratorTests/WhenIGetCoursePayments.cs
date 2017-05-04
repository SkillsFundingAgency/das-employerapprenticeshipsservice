using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.FindEmployerAccountPaymentTransactions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountTransactionOrchestratorTests
{
    class WhenIGetCoursePayments
    {
        private const string HashedAccountId = "123ABC";
        private const string ExternalUser = "Test user";
        private readonly DateTime _fromDate = DateTime.Now.AddDays(-20);
        private readonly DateTime _toDate = DateTime.Now.AddDays(-20);

        private Mock<IMediator> _mediator;
        private EmployerAccountTransactionsOrchestrator _orchestrator;
        private FindEmployerAccountPaymentTransactionsResponse _response;
        private Mock<ICurrentDateTime> _currentTime;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _currentTime = new Mock<ICurrentDateTime>();

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

            _orchestrator = new EmployerAccountTransactionsOrchestrator(_mediator.Object, _currentTime.Object);
        }

        [Test]
        public async Task ThenIShouldGetTotalsByCourseForLevyPayments()
        {
            //Arrange
            var payment1 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 100, TransactionType = TransactionItemType.Payment };
            var payment2 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 50, TransactionType = TransactionItemType.Payment };
            var expectedTotal = payment1.LineAmount + payment2.LineAmount;

            _response = new FindEmployerAccountPaymentTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> { payment1, payment2 }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<FindEmployerAccountPaymentTransactionsQuery>()))
                .ReturnsAsync(_response);

            //Act
            var result = await _orchestrator.GetCoursePayments(HashedAccountId, _fromDate, _toDate, ExternalUser);

            //Assert
            Assert.AreEqual(expectedTotal, result.Data.CoursePayments.First().LevyPaymentAmount);
        }

        [Test]
        public async Task ThenIShouldGetTotalsByCourseForSFACoInvestmentPayments()
        {
            //Arrange
            var payment1 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 100, TransactionType = TransactionItemType.SFACoInvestment };
            var payment2 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 50, TransactionType = TransactionItemType.SFACoInvestment };
            var expectedTotal = payment1.LineAmount + payment2.LineAmount;

            _response = new FindEmployerAccountPaymentTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> { payment1, payment2 }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<FindEmployerAccountPaymentTransactionsQuery>()))
                .ReturnsAsync(_response);

            //Act
            var result = await _orchestrator.GetCoursePayments(HashedAccountId, _fromDate, _toDate, ExternalUser);

            //Assert
            Assert.AreEqual(expectedTotal, result.Data.CoursePayments.First().SFACoInvestmentAmount);
        }

        [Test]
        public async Task ThenIShouldGetTotalsByCourseForEmployerCoInvestmentPayments()
        {
            //Arrange
            var payment1 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 100, TransactionType = TransactionItemType.EmployerCoInvestment };
            var payment2 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 50, TransactionType = TransactionItemType.EmployerCoInvestment };
            var expectedTotal = payment1.LineAmount + payment2.LineAmount;

            _response = new FindEmployerAccountPaymentTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> { payment1, payment2 }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<FindEmployerAccountPaymentTransactionsQuery>()))
                .ReturnsAsync(_response);

            //Act
            var result = await _orchestrator.GetCoursePayments(HashedAccountId, _fromDate, _toDate, ExternalUser);

            //Assert
            Assert.AreEqual(expectedTotal, result.Data.CoursePayments.First().EmployerCoInvestmentAmount);
        }

        [Test]
        public async Task ThenIShouldGetTotalsByCourseForPaymentOverallTotal()
        {
            //Arrange
            var payment1 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 100, TransactionType = TransactionItemType.Payment };
            var payment2 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 90, TransactionType = TransactionItemType.SFACoInvestment };
            var payment3 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 10, TransactionType = TransactionItemType.EmployerCoInvestment };
           
            var expectedTotal = payment1.LineAmount + payment2.LineAmount + payment3.LineAmount;

            _response = new FindEmployerAccountPaymentTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> { payment1, payment2, payment3 }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<FindEmployerAccountPaymentTransactionsQuery>()))
                .ReturnsAsync(_response);

            //Act
            var result = await _orchestrator.GetCoursePayments(HashedAccountId, _fromDate, _toDate, ExternalUser);

            //Assert
            Assert.AreEqual(expectedTotal, result.Data.CoursePayments.First().TotalAmount);
        }

        [Test]
        public async Task ThenIShouldGetTotalsForAllCourses()
        {
            //Arrange
            var payment1 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 100, TransactionType = TransactionItemType.Payment };
            var payment2 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 90, TransactionType = TransactionItemType.SFACoInvestment };
            var payment3 = new PaymentTransactionLine { CourseName = "Test Course", LineAmount = 10, TransactionType = TransactionItemType.EmployerCoInvestment };
            var payment4 = new PaymentTransactionLine { CourseName = "Test Course 2", LineAmount = 100, TransactionType = TransactionItemType.Payment };
            var payment5 = new PaymentTransactionLine { CourseName = "Test Course 2", LineAmount = 90, TransactionType = TransactionItemType.SFACoInvestment };
            var payment6 = new PaymentTransactionLine { CourseName = "Test Course 2", LineAmount = 10, TransactionType = TransactionItemType.EmployerCoInvestment };

            var expectedLevyPaymentsTotal = payment1.LineAmount + payment4.LineAmount;
            var expectedSFACoInvestmentTotal = payment2.LineAmount + payment5.LineAmount;
            var expectedEmployerCoInvestmentTotal = payment3.LineAmount + payment6.LineAmount;
            var expectedPaymentsTotal = payment1.LineAmount + 
                                        payment2.LineAmount + 
                                        payment3.LineAmount +
                                        payment4.LineAmount +
                                        payment5.LineAmount +
                                        payment6.LineAmount;

            _response = new FindEmployerAccountPaymentTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedPaymentsTotal,
                Transactions = new List<PaymentTransactionLine> { payment1, payment2, payment3, payment4, payment5, payment6 }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<FindEmployerAccountPaymentTransactionsQuery>()))
                .ReturnsAsync(_response);

            //Act
            var result = await _orchestrator.GetCoursePayments(HashedAccountId, _fromDate, _toDate, ExternalUser);

            //Assert
            Assert.AreEqual(expectedLevyPaymentsTotal, result.Data.LevyPaymentsTotal);
            Assert.AreEqual(expectedSFACoInvestmentTotal, result.Data.SFACoInvestmentsTotal);
            Assert.AreEqual(expectedEmployerCoInvestmentTotal, result.Data.EmployerCoInvestmentsTotal);
            Assert.AreEqual(expectedPaymentsTotal, result.Data.PaymentsTotal);
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
    }
}
