using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountTransactionsTests
{
    public class WhenIGetEmployerTransactions : QueryBaseTest<GetEmployerAccountTransactionsHandler, GetEmployerAccountTransactionsQuery, GetEmployerAccountTransactionsResponse>
    {
        private Mock<IDasLevyService> _dasLevyService;
        private GetEmployerAccountTransactionsQuery _request;
        private Mock<IApprenticeshipInfoServiceWrapper> _apprenticshipInfoService;
        private Mock<ILogger> _logger;

        public override GetEmployerAccountTransactionsQuery Query { get; set; }
        public override GetEmployerAccountTransactionsHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountTransactionsQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();
          
            _request = new GetEmployerAccountTransactionsQuery
            {
                AccountId = 1,
                HashedAccountId = "RTF34",
                FromDate = DateTime.Now.AddDays(-10),
                ToDate = DateTime.Now.AddDays(10),
                ExternalUserId = "3EFR"
            };

            _dasLevyService = new Mock<IDasLevyService>();
            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(new List<TransactionLine>());

            _dasLevyService.Setup(x => x.GetPreviousAccountTransaction(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(2);

            _apprenticshipInfoService = new Mock<IApprenticeshipInfoServiceWrapper>();

            _logger = new Mock<ILogger>();

            RequestHandler = new GetEmployerAccountTransactionsHandler(_dasLevyService.Object, RequestValidator.Object, _apprenticshipInfoService.Object, _logger.Object);
            Query = new GetEmployerAccountTransactionsQuery();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(_request);

            //Assert
            _dasLevyService.Verify(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var transactions = new List<TransactionLine>
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
            Assert.AreEqual(_request.AccountId, response.Data.AccountId);
            Assert.AreEqual(1, response.Data.TransactionLines.Count);
        }
        

        [Test]
        public async Task ThenIfNoTransactionAreFoundAnEmptyTransactionListIsReturned()
        {
            //Act
            var response = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual(_request.HashedAccountId, response.Data.HashedAccountId);
            Assert.AreEqual(_request.AccountId, response.Data.AccountId);
            Assert.IsEmpty(response.Data.TransactionLines);
        }

        [Test]
        public async Task ThenTheProviderNameIsTakenFromTheService()
        {
            //Arrange
            var expectedUkprn = 545646541;
            var transactions = new List<TransactionLine>
                {
                    new PaymentTransactionLine()
                    {
                        AccountId = 1,
                       TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 1000,
                        TransactionType = TransactionItemType.Payment,
                        UkPrn = expectedUkprn
                    }
                };
            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(transactions);

            _apprenticshipInfoService.Setup(x => x.GetProvider(expectedUkprn)).Returns(new ProvidersView {Provider = new Domain.Models.ApprenticeshipProvider.Provider {ProviderName = "test"}});

            //Act
            await RequestHandler.Handle(_request);

            //Act
            _apprenticshipInfoService.Verify(x=>x.GetProvider(expectedUkprn),Times.Once);
        }

        [Test]
        public async Task ThenTheProviderNameIsSetToUnknownProviderIfTheRecordCantBeFound()
        {
            //Arrange
            var transactions = new List<TransactionLine>
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

            _apprenticshipInfoService.Setup(x => x.GetProvider(It.IsAny<long>())).Throws(new WebException());

            //Act
            var actual = await RequestHandler.Handle(_request);
            
            //Assert
            Assert.AreEqual("Training provider - name not recognised", actual.Data.TransactionLines.First().Description);
            _logger.Verify(x=>x.Info(It.IsAny<Exception>(),"Provider not found for UkPrn:1254545"));
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
                .ReturnsAsync(new List<TransactionLine>
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
                .ReturnsAsync(new List<TransactionLine>
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
            var provider = new Domain.Models.ApprenticeshipProvider.Provider { ProviderName = "test" };
            var transaction = new PaymentTransactionLine
            {
                UkPrn = 100,
                TransactionType = TransactionItemType.Payment,
                Amount = 123.45M
            };

            _apprenticshipInfoService.Setup(x => x.GetProvider(It.IsAny<long>()))
                .Returns(new ProvidersView { Provider = provider });

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<TransactionLine>
                {
                    transaction
                });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First();

            Assert.AreEqual(provider.ProviderName, actualTransaction.Description);
            Assert.AreEqual(transaction.Amount, actualTransaction.Amount);
        }

        [Test]
        public async Task ThenIShouldGetBackCorrectCoInvestmentTransactionFromSFAPayment()
        {
            //Arrange
            var provider = new Domain.Models.ApprenticeshipProvider.Provider { ProviderName = "test" };
            var transaction = new PaymentTransactionLine
            {
                UkPrn = 100,
                TransactionType = TransactionItemType.Payment,
                SfaCoInvestmentAmount = 123.45M
            };

            _apprenticshipInfoService.Setup(x => x.GetProvider(It.IsAny<long>()))
                .Returns(new ProvidersView { Provider = provider });

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<TransactionLine>
                {
                    transaction
                });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First();
            
            Assert.AreEqual($"Co-investment - {provider.ProviderName}", actualTransaction.Description);
            Assert.AreEqual(transaction.Amount, actualTransaction.Amount);
        }

        [Test]
        public async Task ThenIShouldGetBackCorrectCoInvestmentTransactionFromEmployerPayment()
        {
            //Arrange
            var provider = new Domain.Models.ApprenticeshipProvider.Provider { ProviderName = "test" };
            var transaction = new PaymentTransactionLine
            {
                UkPrn = 100,
                TransactionType = TransactionItemType.Payment,
                Amount = 123.45M,
                EmployerCoInvestmentAmount = 50
            };

            _apprenticshipInfoService.Setup(x => x.GetProvider(It.IsAny<long>()))
                .Returns(new ProvidersView { Provider = provider });

            var p = _apprenticshipInfoService.Object.GetProvider(100);

            _dasLevyService.Setup(x => x.GetAccountTransactionsByDateRange(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<TransactionLine>
                {
                    transaction
                });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            var actualTransaction = actual.Data.TransactionLines.First();

            Assert.AreEqual($"Co-investment - {provider.ProviderName}", actualTransaction.Description);
            Assert.AreEqual(transaction.Amount, actualTransaction.Amount);
        }
        
        [Test]
        public async Task ThenShouldReturnPreviousTransactionsAreAvailableIfThereAreSome()
        {
            //Arrange
            var transactions = new List<TransactionLine>
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
            //Arrange
            var transactions = new List<TransactionLine>
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

            _dasLevyService.Setup(x => x.GetPreviousAccountTransaction(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(0);

            //Act
            var result = await RequestHandler.Handle(_request);

            //Assert
            Assert.IsFalse(result.AccountHasPreviousTransactions);
        }
    }
}
