﻿using System;
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
        private GetAccountProviderTransactionsResponse _response;
        private Mock<ICurrentDateTime> _currentTime;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _currentTime = new Mock<ICurrentDateTime>();

            _response = new GetAccountProviderTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = 100,
                Transactions = new List<PaymentTransactionLine>
                {
                    new PaymentTransactionLine {Amount = 100}
                }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountProviderTransactionsQuery>()))
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

            _response = new GetAccountProviderTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> { payment1, payment2 }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountProviderTransactionsQuery>()))
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
            var payment1 = new PaymentTransactionLine { CourseName = "Test Course", SfaCoInvestmentAmount = 100, TransactionType = TransactionItemType.Payment };
            var payment2 = new PaymentTransactionLine { CourseName = "Test Course", SfaCoInvestmentAmount = 50, TransactionType = TransactionItemType.Payment };
            var expectedTotal = payment1.SfaCoInvestmentAmount + payment2.SfaCoInvestmentAmount;

            _response = new GetAccountProviderTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> { payment1, payment2 }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountProviderTransactionsQuery>()))
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
            var payment1 = new PaymentTransactionLine { CourseName = "Test Course", EmployerCoInvestmentAmount = 100, TransactionType = TransactionItemType.Payment };
            var payment2 = new PaymentTransactionLine { CourseName = "Test Course", EmployerCoInvestmentAmount = 50, TransactionType = TransactionItemType.Payment };
            var expectedTotal = payment1.EmployerCoInvestmentAmount + payment2.EmployerCoInvestmentAmount;

            _response = new GetAccountProviderTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> { payment1, payment2 }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountProviderTransactionsQuery>()))
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
            var payment = new PaymentTransactionLine
            {
                CourseName = "Test Course",
                LineAmount = 100,
                SfaCoInvestmentAmount = 90,
                EmployerCoInvestmentAmount = 10,
                TransactionType = TransactionItemType.Payment
            };
          
            var expectedTotal = payment.LineAmount + payment.SfaCoInvestmentAmount + payment.EmployerCoInvestmentAmount;

            _response = new GetAccountProviderTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedTotal,
                Transactions = new List<PaymentTransactionLine> { payment }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountProviderTransactionsQuery>()))
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
            var expectedEmployerCoInvestmentTotal = payment1.EmployerCoInvestmentAmount + payment2.EmployerCoInvestmentAmount;
            var expectedPaymentsTotal = payment1.LineAmount + 
                                        payment1.SfaCoInvestmentAmount + 
                                        payment1.EmployerCoInvestmentAmount +
                                        payment2.LineAmount +
                                        payment2.SfaCoInvestmentAmount +
                                        payment2.EmployerCoInvestmentAmount;

            _response = new GetAccountProviderTransactionsResponse
            {
                ProviderName = "Test Provider",
                TransactionDate = DateTime.Now,
                Total = expectedPaymentsTotal,
                Transactions = new List<PaymentTransactionLine> { payment1, payment2 }
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountProviderTransactionsQuery>()))
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
            _mediator.Verify(x => x.SendAsync(It.Is<GetAccountProviderTransactionsQuery>(
                q => q.HashedAccountId.Equals(HashedAccountId) &&
                     q.FromDate.Equals(_fromDate) &&
                     q.ToDate.Equals(_toDate) &&
                     q.ExternalUserId.Equals(ExternalUser))));
        }

        [Test]
        public async Task ThenIfNoTransactionsAreFoundANotFoundStatusIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountProviderTransactionsQuery>()))
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
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountProviderTransactionsQuery>()))
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
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountProviderTransactionsQuery>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var result = await _orchestrator.FindAccountPaymentTransactions(HashedAccountId, _fromDate, _toDate,
                ExternalUser);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        }
    }
}
