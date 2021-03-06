﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccount;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Orchestrators
{
    public class WhenIGetAccountTransactions
    {
        private const string HashedAccountId = "123ABC";
        private const string ExternalUser = "Test user";
        private const long AccountId = 1234;

        private Mock<IAccountApiClient> _accountApiClient;
        private Mock<IMediator> _mediator;
        private EmployerAccountTransactionsOrchestrator _orchestrator;
        private GetEmployerAccountResponse _response;
        private Mock<ICurrentDateTime> _currentTime;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _accountApiClient = new Mock<IAccountApiClient>();
            _mediator = new Mock<IMediator>();
            _currentTime = new Mock<ICurrentDateTime>();
            _hashingService = new Mock<IHashingService>();

            _response = new GetEmployerAccountResponse
            {
                Account = new Account
                {
                    Name = "Test Account"
                }
            };

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(AccountId);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(_response);

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(AccountId);

            SetupGetTransactionsResponse(2017, 5);

            _orchestrator = new EmployerAccountTransactionsOrchestrator(_accountApiClient.Object, _mediator.Object, _currentTime.Object, Mock.Of<ILog>());
        }

        [Test]
        [TestCase(2, 2017)]
        [TestCase(6, 2017)]
        [TestCase(8, 2019)]
        [TestCase(12, 2020)]
        public async Task ThenARequestShouldBeMadeForTransactions(int month, int year)
        {
            //Act
            await _orchestrator.GetAccountTransactions(HashedAccountId, year, month, ExternalUser);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(
                q => q.Year == year && q.Month == month)), Times.Once);
        }

        [Test]
        public async Task ThenARequestShouldBeMadeForTransactionsForCurrentMonthIfNoYearOrMonthHasBeenGiven()
        {
            //Act
            await _orchestrator.GetAccountTransactions(HashedAccountId, default(int), default(int), ExternalUser);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(
                q => q.Year == 0 && q.Month == 0)), Times.Once);
        }

        [Test]
        public async Task ThenResultShouldHaveYearAndMonthOfRequest()
        {
            //Arrange
            const int year = 2016;
            const int month = 2;
            SetupGetTransactionsResponse(year, month);

            //Act
            var result = await _orchestrator.GetAccountTransactions(HashedAccountId, year, month, ExternalUser);

            //Assert
            Assert.AreEqual(year, result.Data.Year);
            Assert.AreEqual(month, result.Data.Month);
        }

        [Test]
        public async Task ThenResultShouldShowIfTheSelectMonthIsTheLatest()
        {
            //Arrange
            _currentTime.Setup(x => x.Now).Returns(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            SetupGetTransactionsResponse(DateTime.Now.Year, DateTime.Now.Month);

            //Act
            var resultLatestMonth = await _orchestrator.GetAccountTransactions(HashedAccountId, DateTime.Now.Year, DateTime.Now.Month, ExternalUser);
            SetupGetTransactionsResponse(2016, 1);
            var resultHistoricalMonth = await _orchestrator.GetAccountTransactions(HashedAccountId, 2016, 1, ExternalUser);

            //Assert
            Assert.AreEqual(true, resultLatestMonth.Data.IsLatestMonth);
            Assert.AreEqual(false, resultHistoricalMonth.Data.IsLatestMonth);
        }

        [Test]
        public async Task ThenResultShouldHaveWhetherPreviousTransactionsAreAvailable()
        {
            //Act
            var result = await _orchestrator.GetAccountTransactions(HashedAccountId, 2017, 8, ExternalUser);

            //Assert
            Assert.IsTrue(result.Data.AccountHasPreviousTransactions);
        }

        [Test]
        public async Task ThenOnlyOneLevyTransactionShouldBeShownForSummary()
        {
            //Arrange
            var levyTransactions = new List<LevyDeclarationTransactionLine>
            {
                CreateLevyTransaction(new DateTime(2017,5,18), 200),
                CreateLevyTransaction(new DateTime(2017,6,18), 300),
                CreateLevyTransaction(new DateTime(2017,7,18), 500)
            };

            var transactions = new List<TransactionLine>();

            transactions.AddRange(levyTransactions);
            transactions.Add(new PaymentTransactionLine { Amount = 200, TransactionType = TransactionItemType.Payment });

            SetupGetTransactionsResponse(2018, 2, transactions);

            //Act
            var result = await _orchestrator.GetAccountTransactions(HashedAccountId, default(int), default(int), ExternalUser);

            var actualTransactions = result?.Data?.Model?.Data?.TransactionLines;

            var levyDeclaration =
                actualTransactions?.SingleOrDefault(t => t.TransactionType == TransactionItemType.Declaration);

            //Assert
            Assert.IsNotNull(levyDeclaration);
        }

        [Test]
        public async Task ThenLevyAggregationShouldNotAffectOtherTransactions()
        {
            //Arrange
            var levyTransactions = new List<LevyDeclarationTransactionLine>
            {
                CreateLevyTransaction(new DateTime(2017,5,18), 200),
                CreateLevyTransaction(new DateTime(2017,6,18), 300),
                CreateLevyTransaction(new DateTime(2017,7,18), 500)
            };

            var transactions = new List<TransactionLine>();

            transactions.AddRange(levyTransactions);
            transactions.Add(new PaymentTransactionLine { Amount = 200, TransactionType = TransactionItemType.Payment });

            SetupGetTransactionsResponse(2018, 2, transactions);

            //Act
            var result = await _orchestrator.GetAccountTransactions(HashedAccountId, default(int), default(int), ExternalUser);

            var actualTransactions = result?.Data?.Model?.Data?.TransactionLines;

            var paymenTransaction =
                actualTransactions?.SingleOrDefault(t => t.TransactionType == TransactionItemType.Payment);

            //Assert
            Assert.IsNotNull(paymenTransaction);
            Assert.AreEqual(2, actualTransactions.Length);
        }

        [Test]
        public async Task ThenAggregatedLevyTransactionShouldHaveCorrectDescription()
        {
            //Arrange
            var levyTransactions = new List<LevyDeclarationTransactionLine>
            {
                CreateLevyTransaction(new DateTime(2017,5,18), 200),
                CreateLevyTransaction(new DateTime(2017,6,18), 300),
                CreateLevyTransaction(new DateTime(2017,7,18), 500)
            };

            var transactions = new List<TransactionLine>();

            transactions.AddRange(levyTransactions);
            transactions.Add(new PaymentTransactionLine { Amount = 200, TransactionType = TransactionItemType.Payment });

            SetupGetTransactionsResponse(2018, 2, transactions);

            //Act
            var result = await _orchestrator.GetAccountTransactions(HashedAccountId, default(int), default(int), ExternalUser);

            var actualTransactions = result?.Data?.Model?.Data?.TransactionLines;

            var levyDeclaration =
                actualTransactions?.SingleOrDefault(t => t.TransactionType == TransactionItemType.Declaration);

            //Assert
            Assert.AreEqual(levyTransactions.First().Description, levyDeclaration?.Description);
        }

        [Test]
        public async Task ThenAggregatedLevyTransactionShouldHaveCorrectAmount()
        {
            //Arrange           
            var levyTransactions = new List<LevyDeclarationTransactionLine>
            {
                CreateLevyTransaction(new DateTime(2017,5,18), 200),
                CreateLevyTransaction(new DateTime(2017,6,18), 300),
                CreateLevyTransaction(new DateTime(2017,7,18), 500)
            };

            var transactions = new List<TransactionLine>();

            transactions.AddRange(levyTransactions);
            transactions.Add(new PaymentTransactionLine { Amount = 200, TransactionType = TransactionItemType.Payment });

            SetupGetTransactionsResponse(2018, 2, transactions);

            //Act
            var result = await _orchestrator.GetAccountTransactions(HashedAccountId, default(int), default(int), ExternalUser);

            var actualTransactions = result?.Data?.Model?.Data?.TransactionLines;

            //Assert
            Assert.IsNotNull(actualTransactions);
            Assert.AreEqual(levyTransactions.Sum(t => t.Amount), actualTransactions.Single(t => t.TransactionType == TransactionItemType.Declaration).Amount);
        }

        private void SetupGetTransactionsResponse(int year, int month)
        {
            SetupGetTransactionsResponse(year, month, new TransactionLine[0]);
        }

        private void SetupGetTransactionsResponse(int year, int month, IEnumerable<TransactionLine> transactions)
        {
            _accountApiClient.Setup(s => s.GetAccount(HashedAccountId))
            .Returns(Task.FromResult(
                new EAS.Account.Api.Types.AccountDetailViewModel
                {
                    HashedAccountId = HashedAccountId,
                    AccountId = AccountId
                })
            );

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountTransactionsQuery>()))
                .ReturnsAsync(new GetEmployerAccountTransactionsResponse
                {
                    Data = new AggregationData
                    {
                        TransactionLines = transactions.ToArray()
                    },
                    Year = year,
                    Month = month,
                    AccountHasPreviousTransactions = true
                });
        }

        private static LevyDeclarationTransactionLine CreateLevyTransaction(DateTime submissionDate, int amount)
        {
            return new LevyDeclarationTransactionLine
            {
                Amount = amount,
                DateCreated = DateTime.Now,
                TransactionDate = submissionDate,
                TransactionType = TransactionItemType.Declaration,
                Description = "Levy"
            };
        }
    }
}
