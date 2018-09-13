using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.FindAccountProviderPayments;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Orchestrators
{
    internal class WhenIGetProviderPayments
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

        private Expression<Func<IMediator, Task<FindAccountProviderPaymentsResponse>>> AssertExpressionValidation()
        {
            return x => x.SendAsync(It.Is<FindAccountProviderPaymentsQuery>(c => c.ExternalUserId.Equals(ExternalUser)
                                                                                 && c.FromDate.Equals(_fromDate)
                                                                                 && c.ToDate.Equals(_toDate)
                                                                                 && c.HashedAccountId.Equals(
                                                                                     HashedAccountId)
                                                                                 && c.UkPrn.Equals(ExpectedUkPrn)));
        }


        [Test]
        public async Task ThenIShouldGetTotalsByCourseForSFACoInvestmentPayments()
        {
            //Arrange
            var payment1 = new PaymentTransactionLine
            {
                CourseName = "Test Course",
                SfaCoInvestmentAmount = 100,
                TransactionType = TransactionItemType.Payment
            };
            var payment2 = new PaymentTransactionLine
            {
                CourseName = "Test Course",
                SfaCoInvestmentAmount = 50,
                TransactionType = TransactionItemType.Payment
            };
            var expectedTotal = payment1.SfaCoInvestmentAmount + payment2.SfaCoInvestmentAmount;

            _response = new FindAccountProviderPaymentsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> {payment1, payment2}
            };
            _mediator.Setup(AssertExpressionValidation()).ReturnsAsync(_response);

            //Act
            var result = await _orchestrator.GetProviderPaymentSummary(HashedAccountId, ExpectedUkPrn, _fromDate,
                _toDate, ExternalUser);

            //Assert
            Assert.AreEqual(expectedTotal, result.Data.CoursePayments.First().SFACoInvestmentAmount);
        }

        [Test]
        public async Task ThenIShouldGetTotalsByCourseForEmployerCoInvestmentPayments()
        {
            //Arrange
            var payment1 = new PaymentTransactionLine
            {
                CourseName = "Test Course",
                EmployerCoInvestmentAmount = 100,
                TransactionType = TransactionItemType.Payment
            };
            var payment2 = new PaymentTransactionLine
            {
                CourseName = "Test Course",
                EmployerCoInvestmentAmount = 50,
                TransactionType = TransactionItemType.Payment
            };
            var expectedTotal = payment1.EmployerCoInvestmentAmount + payment2.EmployerCoInvestmentAmount;

            _response = new FindAccountProviderPaymentsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> {payment1, payment2}
            };
            _mediator.Setup(AssertExpressionValidation()).ReturnsAsync(_response);

            //Act
            var result = await _orchestrator.GetProviderPaymentSummary(HashedAccountId, ExpectedUkPrn, _fromDate,
                _toDate, ExternalUser);

            //Assert
            Assert.AreEqual(expectedTotal, result.Data.CoursePayments.First().EmployerCoInvestmentAmount);
        }


        [Test]
        public async Task ThenIShouldGetTotalsByCourseForPaymentOverallTotal()
        {
            //Arrange
            var payment = new PaymentTransactionLine
            {
                CourseName = "Test Course",
                LineAmount = 100,
                SfaCoInvestmentAmount = 90,
                EmployerCoInvestmentAmount = 10,
                TransactionType = TransactionItemType.Payment
            };

            var expectedTotal = payment.LineAmount + payment.SfaCoInvestmentAmount + payment.EmployerCoInvestmentAmount;

            _response = new FindAccountProviderPaymentsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> {payment}
            };
            _mediator.Setup(AssertExpressionValidation()).ReturnsAsync(_response);

            //Act
            var result = await _orchestrator.GetProviderPaymentSummary(HashedAccountId, ExpectedUkPrn, _fromDate,
                _toDate, ExternalUser);

            //Assert
            Assert.AreEqual(expectedTotal, result.Data.CoursePayments.First().TotalAmount);
        }


        [Test]
        public async Task ThenIShouldGetTotalsForAllCourses()
        {
            //Arrange
            var payment1 = new PaymentTransactionLine
            {
                CourseName = "Test Course",
                LineAmount = 100,
                SfaCoInvestmentAmount = 90,
                EmployerCoInvestmentAmount = 10,
                TransactionType = TransactionItemType.Payment
            };

            var payment2 = new PaymentTransactionLine
            {
                CourseName = "Test Course 2",
                LineAmount = 100,
                SfaCoInvestmentAmount = 90,
                EmployerCoInvestmentAmount = 10,
                TransactionType = TransactionItemType.Payment
            };

            var expectedLevyPaymentsTotal = payment1.LineAmount + payment2.LineAmount;
            var expectedSFACoInvestmentTotal = payment1.SfaCoInvestmentAmount + payment2.SfaCoInvestmentAmount;
            var expectedEmployerCoInvestmentTotal =
                payment1.EmployerCoInvestmentAmount + payment2.EmployerCoInvestmentAmount;
            var expectedPaymentsTotal = payment1.LineAmount +
                                        payment1.SfaCoInvestmentAmount +
                                        payment1.EmployerCoInvestmentAmount +
                                        payment2.LineAmount +
                                        payment2.SfaCoInvestmentAmount +
                                        payment2.EmployerCoInvestmentAmount;

            _response = new FindAccountProviderPaymentsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedPaymentsTotal,
                Transactions = new List<PaymentTransactionLine> {payment1, payment2}
            };
            _mediator.Setup(AssertExpressionValidation()).ReturnsAsync(_response);

            //Act
            var result = await _orchestrator.GetProviderPaymentSummary(HashedAccountId, ExpectedUkPrn, _fromDate,
                _toDate, ExternalUser);

            //Assert
            Assert.AreEqual(expectedLevyPaymentsTotal, result.Data.LevyPaymentsTotal);
            Assert.AreEqual(expectedSFACoInvestmentTotal, result.Data.SFACoInvestmentsTotal);
            Assert.AreEqual(expectedEmployerCoInvestmentTotal, result.Data.EmployerCoInvestmentsTotal);
            Assert.AreEqual(expectedPaymentsTotal, result.Data.PaymentsTotal);
        }
    }
}