﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.GetAccountLevyTransactions;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetAccountLevyTransactionDetailTests
{
    /// <summary>
    ///  AML-2454: Move to finance
    /// </summary>
    public class WhenIGetAccountLevyTransactionDetails : QueryBaseTest<GetAccountLevyTransactionsQueryHandler, GetAccountLevyTransactionsQuery, GetAccountLevyTransactionsResponse>
    {
        private Mock<ITransactionRepository> _transactionRepository;
        private DateTime _fromDate;
        private DateTime _toDate;
        private long _accountId;
        private Mock<IHmrcDateService> _hmrcDataService;
        public override GetAccountLevyTransactionsQuery Query { get; set; }
        public override GetAccountLevyTransactionsQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountLevyTransactionsQuery>> RequestValidator { get; set; }
      
        [SetUp]
        public void Arrange()
        {
            _hmrcDataService = new Mock<IHmrcDateService>();

            _fromDate = DateTime.Now.AddDays(-10);
            _toDate = DateTime.Now.AddDays(-2);
            _accountId = 1;

            _transactionRepository = new Mock<ITransactionRepository>();
            _transactionRepository.Setup(x => x.GetAccountLevyTransactionsByDateRange(
                    It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                            .ReturnsAsync(new List<TransactionLine>
                            {
                                new LevyDeclarationTransactionLine {PayrollMonth = 1,PayrollYear = "16-17"}
                            });

            Query = new GetAccountLevyTransactionsQuery
            {
                AccountId = _accountId,
                FromDate= _fromDate,
                ToDate = _toDate
            };

            SetUp();

            RequestHandler = new GetAccountLevyTransactionsQueryHandler(RequestValidator.Object, _transactionRepository.Object, _hmrcDataService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _transactionRepository.Verify(x=>x.GetAccountLevyTransactionsByDateRange(Query.AccountId, Query.FromDate, Query.ToDate));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual.Transactions);
        }

        [Test]
        public async Task ThenThePayrollDateIsCorrectlyMapped()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _hmrcDataService.Verify(x=>x.GetDateFromPayrollYearMonth(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }
    }
}
