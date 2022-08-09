using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Models.Transfers;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.MarkerInterfaces;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetEmployerAccountTransactionsTests
{
    [ExcludeFromCodeCoverage]
    public class WhenIGetEmployerTransactions : QueryBaseTest<GetEmployerAccountTransactionsHandler, GetEmployerAccountTransactionsQuery, GetEmployerAccountTransactionsResponse>
    {
        private Mock<IDasLevyService> _dasLevyService;
        private GetEmployerAccountTransactionsQuery _request;
        private Mock<ILog> _logger;
        public override GetEmployerAccountTransactionsQuery Query { get; set; }
        public override GetEmployerAccountTransactionsHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountTransactionsQuery>> RequestValidator { get; set; }
        private Mock<IHashingService> _hashingService;
        private Mock<IPublicHashingService> _publicHashingService;


        [SetUp]
        public void Arrange()
        {
            SetUp();

            _request = new GetEmployerAccountTransactionsQuery
            {
                HashedAccountId = "RTF34",
                ExternalUserId = "3EFR"
            };

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(_request.HashedAccountId)).Returns(1);

            _publicHashingService = new Mock<IPublicHashingService>();

            _dasLevyService = new Mock<IDasLevyService>();
            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(new TransactionLine[0]);

            _dasLevyService.Setup(x => x.GetPreviousAccountTransaction(It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(2);

             _logger = new Mock<ILog>();

            RequestHandler = new GetEmployerAccountTransactionsHandler(
                _dasLevyService.Object,
                RequestValidator.Object,
                _logger.Object,
                _hashingService.Object,
                _publicHashingService.Object);
            Query = new GetEmployerAccountTransactionsQuery();
        }

        //[Test]
        //public void ThenIfTheUserIsNotAuthorisedAnExceptionIsThrown()
        //{
        //    //Arrange
        //    RequestValidator.Setup(x => x.ValidateAsync(_request)).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

        //    //Act
        //    Assert.ThrowsAsync<UnauthorizedAccessException>(() => RequestHandler.Handle(_request));
        //}

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            _request.Year = 0;
            _request.Month = 0;

            var daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, daysInMonth);

            //Act
            await RequestHandler.Handle(_request);

            //Assert
            _dasLevyService.Verify(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), fromDate, toDate), Times.Once);
        }

        [Test]
        public async Task ThenIfAMonthIsProvidedTheRepositoryIsCalledForThatMonthMonth()
        {
            //Arrange
            _request.Year = 2017;
            _request.Month = 3;

            var daysInMonth = DateTime.DaysInMonth(_request.Year, _request.Month);

            var fromDate = new DateTime(_request.Year, _request.Month, 1);
            var toDate = new DateTime(_request.Year, _request.Month, daysInMonth);

            //Act
            await RequestHandler.Handle(_request);

            //Assert
            _dasLevyService.Verify(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), fromDate, toDate), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var transactions = new TransactionLine[]
                {
                    new LevyDeclarationTransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 1,
                        TransactionDate = DateTime.Now.AddDays(-3),
                        Amount = 1000,
                        TransactionType = TransactionItemType.TopUp,
                        EmpRef = "123"
                    }
                };

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(transactions);

            //Act
            var response = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual(_request.HashedAccountId, response.Data.HashedAccountId);
            Assert.AreEqual(1, response.Data.AccountId);
            Assert.AreEqual(1, response.Data.TransactionLines.Length);
        }


        [Test]
        public async Task ThenIfNoTransactionAreFoundAnEmptyTransactionListIsReturned()
        {
            //Act
            var response = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual(_request.HashedAccountId, response.Data.HashedAccountId);
            Assert.AreEqual(1, response.Data.AccountId);
            Assert.IsEmpty(response.Data.TransactionLines);
        }

        [Test]
        public async Task ThenTheProviderNameIsTakenFromTheService()
        {
            //Arrange
            var expectedUkprn = 545646541;
            var transactions = new TransactionLine[]
                {
                    new PaymentTransactionLine()
                    {
                        AccountId = 1,
                       TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 1000,
                        TransactionType = TransactionItemType.Payment,
                        UkPrn = expectedUkprn,
                        PeriodEnd = "17-18"
                    }
                };
            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(transactions);

             _dasLevyService.Setup(x => x.GetProviderName(expectedUkprn, It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync("test");
            //Act
            var actual = await RequestHandler.Handle(_request);

            //Assert
            _dasLevyService.Verify(x => x.GetProviderName(expectedUkprn, It.IsAny<long>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenTheProviderNameIsNotRecognisedIfTheRecordThrowsAndException()
        {
            //Arrange
            var transactions = new TransactionLine[]
                {
                    new PaymentTransactionLine
                    {
                        AccountId = 1,
                        TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 1000,
                        TransactionType = TransactionItemType.Payment,
                        UkPrn = 1254545
                    }
                };
            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(transactions);

            _dasLevyService.Setup(x => x.GetProviderName(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>())).Throws(new WebException());

            //Act
            var actual = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual("Training provider - name not recognised", actual.Data.TransactionLines.First().Description);
            _logger.Verify(x => x.Info(It.Is<string>(y => y.StartsWith("Provider not found for UkPrn:1254545"))));
        }

        [Test]
        public async Task ThenTheProviderNameIsSetToUnknownProviderIfTheRecordCantBeFound()
        {
            //Arrange
            var transactions = new TransactionLine[]
            {
                new PaymentTransactionLine
                {
                    AccountId = 1,
                    TransactionDate = DateTime.Now.AddMonths(-3),
                    Amount = 1000,
                    TransactionType = TransactionItemType.Payment,
                    UkPrn = 1254545
                }
            };
            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transactions);

            _dasLevyService.Setup(x => x.GetProviderName(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync((string)null);

            //Act
            var actual = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual("Training provider - name not recognised", actual.Data.TransactionLines.First().Description);
        }

        [Test]
        public async Task ThenShouldLogIfExceptionOccursWhenGettingProviderName()
        {
            //Arrange
            var transactions = new TransactionLine[]
            {
                new PaymentTransactionLine
                {
                    AccountId = 1,
                    TransactionDate = DateTime.Now.AddMonths(-3),
                    Amount = 1000,
                    TransactionType = TransactionItemType.Payment,
                    UkPrn = 1254545
                }
            };

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transactions);

            _dasLevyService.Setup(x => x.GetProviderName(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
                .Throws<Exception>();

            //Act
            await RequestHandler.Handle(_request);

            //Assert
            _logger.Verify(x => x.Info(It.Is<string>(y => y.StartsWith("Provider not found for UkPrn:1254545"))));
        }

        [Test]
        public async Task ThenIShouldGetBackCorrectLevyTransactions()
        {
            //Arrange
            var transaction = new LevyDeclarationTransactionLine
            {
                TransactionType = TransactionItemType.Declaration,
                Amount = 123.45M
            };

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TransactionLine[]
                {
                    transaction
                });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First();

            Assert.AreEqual("Levy", actualTransaction.Description);
            Assert.AreEqual(transaction.Amount, actualTransaction.Amount);
        }


        [Test]
        public async Task ThenIShouldGetBackCorrectLevyAdjustmentTransactions()
        {
            //Arrange
            var transaction = new LevyDeclarationTransactionLine
            {
                TransactionType = TransactionItemType.Declaration,
                Amount = -100.50M
            };

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TransactionLine[]
                {
                    transaction
                });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First();

            Assert.AreEqual("Levy adjustment", actualTransaction.Description);
            Assert.AreEqual(transaction.Amount, actualTransaction.Amount);
        }

        [Test]
        public async Task ThenIShouldGetBackCorrectPaymentTransactions()
        {
            //Arrange
            var provider = new EmployerFinance.Models.ApprenticeshipProvider.Provider { Name = "test" };
            var transaction = new PaymentTransactionLine
            {
                UkPrn = 100,
                TransactionType = TransactionItemType.Payment,
                Amount = 123.45M
            };
      
            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TransactionLine[]
                {
                    transaction
                });

            _dasLevyService.Setup(x => x.GetProviderName(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(provider.Name);

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First();

            Assert.AreEqual(provider.Name, actualTransaction.Description);
            Assert.AreEqual(transaction.Amount, actualTransaction.Amount);
        }

        [Test]
        public async Task ThenIShouldGetBackCorrectCoInvestmentTransactionFromSFAPayment()
        {
            //Arrange
            var provider = new EmployerFinance.Models.ApprenticeshipProvider.Provider { Name = "test" };
            var transaction = new PaymentTransactionLine
            {
                UkPrn = 100,
                TransactionType = TransactionItemType.Payment,
                SfaCoInvestmentAmount = 123.45M
            };

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TransactionLine[]
                {
                    transaction
                });

            _dasLevyService.Setup(x => x.GetProviderName(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(provider.Name);

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First();

            Assert.AreEqual($"Co-investment - {provider.Name}", actualTransaction.Description);
            Assert.AreEqual(transaction.Amount, actualTransaction.Amount);
        }

        [Test]
        public async Task ThenIShouldGetBackCorrectCoInvestmentTransactionFromEmployerPayment()
        {
            //Arrange
            var provider = new EmployerFinance.Models.ApprenticeshipProvider.Provider { Name = "test" };
            var transaction = new PaymentTransactionLine
            {
                UkPrn = 100,
                TransactionType = TransactionItemType.Payment,
                Amount = 123.45M,
                EmployerCoInvestmentAmount = 50
            };

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TransactionLine[]
                {
                    transaction
                });

            _dasLevyService.Setup(x => x.GetProviderName(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(provider.Name);

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First();

            Assert.AreEqual($"Co-investment - {provider.Name}", actualTransaction.Description);
            Assert.AreEqual(transaction.Amount, actualTransaction.Amount);
        }

        [Test]
        public async Task ThenShouldReturnPreviousTransactionsAreAvailableIfThereAreSome()
        {
            //Arrange
            var transactions = new TransactionLine[]
            {
                new LevyDeclarationTransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 1,
                    TransactionDate = DateTime.Now.AddDays(-3),
                    Amount = 1000,
                    TransactionType = TransactionItemType.TopUp,
                    EmpRef = "123"
                }
            };

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transactions);

            //Act
            var result = await RequestHandler.Handle(_request);

            //Assert
            Assert.IsTrue(result.AccountHasPreviousTransactions);
        }

        [Test]
        public async Task ThenShouldReturnPreviousTransactionsAreNotAvailableIfThereAreNone()
        {
            //Arrange
            var transactions = new TransactionLine[]
            {
                new LevyDeclarationTransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 1,
                    TransactionDate = DateTime.Now.AddDays(-3),
                    Amount = 1000,
                    TransactionType = TransactionItemType.TopUp,
                    EmpRef = "123"
                }
            };

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transactions);

            _dasLevyService.Setup(x => x.GetPreviousAccountTransaction(It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(0);

            //Act
            var result = await RequestHandler.Handle(_request);

            //Assert
            Assert.IsFalse(result.AccountHasPreviousTransactions);
        }

        [Test]
        public async Task ThenIShouldGetBackCorrectTransferTransactions()
        {
            //Arrange
            var transaction = new TransferTransactionLine
            {
                ReceiverAccountName = "Test Corp",
                TransactionType = TransactionItemType.Transfer,
                Amount = 2035.20M
            };

            var expectedDescription = $"Transfer sent to {transaction.ReceiverAccountName}";

            _dasLevyService.Setup(x =>
                    x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TransactionLine[]
                {
                    transaction
                });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First();

            Assert.AreEqual(expectedDescription, actualTransaction.Description);
            Assert.AreEqual(transaction.Amount, actualTransaction.Amount);
        }

        [Test]
        public async Task ThenIShouldGetTransferReceiverPublicHashedId()
        {
            //Arrange
            var expectedPublicHashedId = "TTT222";

            var transaction = new TransferTransactionLine
            {
                ReceiverAccountId = 3,
                ReceiverAccountName = "Test Corp",
                TransactionType = TransactionItemType.Transfer,
                Amount = 2035.20M
            };

            _dasLevyService.Setup(x =>
                    x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TransactionLine[]
                {
                    transaction
                });

            _publicHashingService.Setup(x => x.HashValue(transaction.ReceiverAccountId))
                .Returns(expectedPublicHashedId);

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First() as TransferTransactionLine;

            Assert.AreEqual(expectedPublicHashedId, actualTransaction?.ReceiverAccountPublicHashedId);
        }

        [Test]
        public async Task ThenIShouldGetBackCorrectExpiredFundTransactions()
        {
            //Arrange
            var transaction = new ExpiredFundTransactionLine
            {
                TransactionType = TransactionItemType.ExpiredFund,
                Amount = 2035.20M
            };

            var expectedDescription = "Expired levy";

            _dasLevyService.Setup(x =>
                    x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TransactionLine[]
                {
                    transaction
                });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First();

            Assert.AreEqual(expectedDescription, actualTransaction.Description);
            Assert.AreEqual(transaction.Amount, actualTransaction.Amount);
        }
    }
}
