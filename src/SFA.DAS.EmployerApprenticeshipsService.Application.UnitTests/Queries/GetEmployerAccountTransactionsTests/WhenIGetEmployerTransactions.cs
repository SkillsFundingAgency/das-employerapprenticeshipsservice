using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountTransactionsTests
{
    public class WhenIGetEmployerTransactions : QueryBaseTest<GetEmployerAccountTransactionsHandler, GetEmployerAccountTransactionsQuery, GetEmployerAccountTransactionsResponse>
    {
        private Mock<IDasLevyService> _dasLevyService;
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private List<AccountHistoryEntry> _accountHistoryList;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _accountHistoryList = new List<AccountHistoryEntry>();
            _dasLevyService = new Mock<IDasLevyService>();
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(new List<TransactionLine> {new TransactionLine()});
            _employerAccountRepository.Setup(x => x.GetAccountHistory(It.IsAny<long>())).ReturnsAsync(_accountHistoryList);

            RequestHandler = new GetEmployerAccountTransactionsHandler(_dasLevyService.Object, RequestValidator.Object, _employerAccountRepository.Object);
            Query = new GetEmployerAccountTransactionsQuery();
        }
        public override GetEmployerAccountTransactionsQuery Query { get; set; }
        public override GetEmployerAccountTransactionsHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountTransactionsQuery>> RequestValidator { get; set; }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            var expectedAcountId = 1;
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAccountTransactionsQuery>())).ReturnsAsync(new ValidationResult());

            //Act
            await RequestHandler.Handle(new GetEmployerAccountTransactionsQuery { 
                    AccountId = expectedAcountId,
                    ExternalUserId = "3EFR",
                    HashedId = "RTF34"
                });

            //Assert
            _dasLevyService.Verify(x=>x.GetTransactionsByAccountId(expectedAcountId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var expectedAcountId = 1;
            var expectedHashedId = "RTF34";
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAccountTransactionsQuery>())).ReturnsAsync(new ValidationResult());

            //Act
            var response = await RequestHandler.Handle(new GetEmployerAccountTransactionsQuery
            {
                AccountId = expectedAcountId,
                ExternalUserId = "3EFR",
                HashedId = expectedHashedId
            });

            //Assert
            Assert.AreEqual(expectedHashedId, response.Data.HashedId);
            Assert.AreEqual(expectedAcountId, response.Data.AccountId);
            Assert.AreEqual(1,response.Data.TransactionSummary.Count);
        }

        [Test]
        public async Task ThenTheTransactionSummaryIsAggregatedFromTheLines()
        {
            //Arrange
            var expectedAcountId = 1;
            var expectedHashedId = "RTF34";
            _dasLevyRepository.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(new List<TransactionLine> {
                new TransactionLine
                {
                    AccountId = expectedAcountId,
                    Amount=500,
                    SubmissionId = 101,
                    TransactionDate = new DateTime(2016,01,10),
                    TransactionType = LevyItemType.Declaration
                },
                new TransactionLine
                {
                    AccountId = expectedAcountId,
                    Amount=50,
                    SubmissionId = 101,
                    TransactionDate = new DateTime(2016,01,10),
                    TransactionType = LevyItemType.TopUp
                } });

            //Act
            var response = await RequestHandler.Handle(new GetEmployerAccountTransactionsQuery
            {
                AccountId = expectedAcountId,
                ExternalUserId = "3EFR",
                HashedId = expectedHashedId
            });

            //Assert
            Assert.IsAssignableFrom<List<TransactionSummary>>(response.Data.TransactionSummary);
            var transactionLine = response.Data.TransactionSummary.FirstOrDefault();
            Assert.IsNotNull(transactionLine);
            Assert.AreEqual(550,transactionLine.Amount);
            Assert.AreEqual(550,transactionLine.Balance);
        }

        [Test]
        public async Task ThenTheBalanceIsCalculatedAsARunningTotal()
        {
            //Arrange
            var expectedAcountId = 1;
            var expectedHashedId = "RTF34";
            _dasLevyRepository.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(new List<TransactionLine> {
                new TransactionLine
                {
                    AccountId = expectedAcountId,
                    Amount=500,
                    SubmissionId = 102,
                    TransactionDate = new DateTime(2016,02,10),
                    TransactionType = LevyItemType.Declaration
                },
                new TransactionLine
                {
                    AccountId = expectedAcountId,
                    Amount=200,
                    SubmissionId = 101,
                    TransactionDate = new DateTime(2016,01,10),
                    TransactionType = LevyItemType.Declaration
                },
                new TransactionLine
                {
                    AccountId = expectedAcountId,
                    Amount=50,
                    SubmissionId = 101,
                    TransactionDate = new DateTime(2016,01,10),
                    TransactionType = LevyItemType.TopUp
                } });


            //Act
            var response = await RequestHandler.Handle(new GetEmployerAccountTransactionsQuery
            {
                AccountId = expectedAcountId,
                ExternalUserId = "3EFR",
                HashedId = expectedHashedId
            });

            //Assert
            Assert.AreEqual(750, response.Data.TransactionSummary[0].Balance);
            Assert.AreEqual(250, response.Data.TransactionSummary[1].Balance);
        }

        [Test]
        public async Task ThenTheCorrectDescriptionIsDisplayedForPositiveAndNegativeAmounts()
        {
            //Arrange
            var expectedAcountId = 1;
            var expectedHashedId = "RTF34";
            _dasLevyRepository.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(new List<TransactionLine> {
                new TransactionLine
                {
                    AccountId = expectedAcountId,
                    Amount=500,
                    SubmissionId = 102,
                    TransactionDate = new DateTime(2016,02,10),
                    TransactionType = LevyItemType.Declaration
                },
                new TransactionLine
                {
                    AccountId = expectedAcountId,
                    Amount=-200,
                    SubmissionId = 101,
                    TransactionDate = new DateTime(2016,01,10),
                    TransactionType = LevyItemType.Declaration
                } });

            //Act
            var response = await RequestHandler.Handle(new GetEmployerAccountTransactionsQuery
            {
                AccountId = expectedAcountId,
                ExternalUserId = "3EFR",
                HashedId = expectedHashedId
            });

            //Assert
            Assert.AreEqual("Credit", response.Data.TransactionSummary[0].Description);
            Assert.AreEqual("Adjustment", response.Data.TransactionSummary[1].Description);
        }

        [Test]
        public async Task ThenTheTransactionDateIsAlwaysTheTwnentiethOfTheMonth()
        {
            //Arrange
            var expectedAcountId = 1;
            var expectedHashedId = "RTF34";
            _dasLevyRepository.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(new List<TransactionLine> {
                new TransactionLine
                {
                    AccountId = expectedAcountId,
                    Amount=500,
                    SubmissionId = 102,
                    TransactionDate = new DateTime(2016,02,10),
                    TransactionType = LevyItemType.Declaration
                },
                new TransactionLine
                {
                    AccountId = expectedAcountId,
                    Amount=-200,
                    SubmissionId = 101,
                    TransactionDate = new DateTime(2016,01,10),
                    TransactionType = LevyItemType.Declaration
                } });

            //Act
            var response = await RequestHandler.Handle(new GetEmployerAccountTransactionsQuery
            {
                AccountId = expectedAcountId,
                ExternalUserId = "3EFR",
                HashedId = expectedHashedId
            });

            //Assert
            Assert.AreEqual(20, response.Data.TransactionSummary[0].TransactionDate.Day);
            Assert.AreEqual(20, response.Data.TransactionSummary[1].TransactionDate.Day);
        }

        [Test]
        public async Task ThenTheTransactionsWillHaveACorrectRunningBalance()
        {
            //Arrange
            var transactions = new List<TransactionLine>()
            {
                new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 1,
                    TransactionDate = DateTime.Now.AddDays(-10),
                    Amount = 1000,
                    TransactionType = LevyItemType.TopUp
                },
                new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 2,
                    TransactionDate = DateTime.Now.AddDays(-8),
                    Amount = -500,
                    TransactionType = LevyItemType.Declaration
                },
                new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 3,
                    TransactionDate = DateTime.Now.AddDays(-2),
                    Amount = 5000,
                    TransactionType = LevyItemType.Declaration
                }
            };

            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAccountTransactionsQuery>())).ReturnsAsync(new ValidationResult());
            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(transactions);


            //Act
            var response = await RequestHandler.Handle(new GetEmployerAccountTransactionsQuery
            {
                AccountId = 1,
                ExternalUserId = "3EFR",
                HashedId = "123"
            });

            //Assert
            Assert.AreEqual(3, response.Data.TransactionLines.Count);
            Assert.AreEqual(1000, response.Data.TransactionLines.Single(x => x.SubmissionId.Equals(1)).Balance);
            Assert.AreEqual(500, response.Data.TransactionLines.Single(x => x.SubmissionId.Equals(2)).Balance);
            Assert.AreEqual(5500, response.Data.TransactionLines.Single(x => x.SubmissionId.Equals(3)).Balance);
        }

        [Test]
        public async Task ThenTransactionThatWereAddedBeforeSchemaIsAddedToAccountAreAggregated()
        {
            //Arrange
            var transactions = new List<TransactionLine>
            {
                new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 1,
                    TransactionDate = DateTime.Now.AddDays(-10),
                    Amount = 1000,
                    TransactionType = LevyItemType.TopUp,
                    EmpRef = "123"
                },
                new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 2,
                    TransactionDate = DateTime.Now.AddDays(-8),
                    Amount = -500,
                    TransactionType = LevyItemType.Declaration,
                    EmpRef = "123"
                },
                new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 3,
                    TransactionDate = DateTime.Now.AddDays(-2),
                    Amount = 5000,
                    TransactionType = LevyItemType.Declaration,
                    EmpRef = "123"
                },
                 new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 4,
                    TransactionDate = DateTime.Now.AddDays(-1),
                    Amount = 2000,
                    TransactionType = LevyItemType.Declaration,
                    EmpRef = "123"
                }
            };

            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAccountTransactionsQuery>())).ReturnsAsync(new ValidationResult());
            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(transactions);
            
            _accountHistoryList.Add(new AccountHistoryEntry
            {
                AccountId = 1,
                DateAdded = DateTime.Now.AddDays(-3)
            });

            //Act
            var response = await RequestHandler.Handle(new GetEmployerAccountTransactionsQuery
            {
                AccountId = 1,
                ExternalUserId = "3EFR",
                HashedId = "123"
            });

            //Assert
            Assert.AreEqual(3, response.Data.TransactionLines.Count);
            Assert.AreEqual(500, response.Data.TransactionLines.Single(x => x.SubmissionId.Equals(0)).Balance);
            Assert.AreEqual(5500, response.Data.TransactionLines.Single(x => x.SubmissionId.Equals(3)).Balance);
            Assert.AreEqual(7500, response.Data.TransactionLines.Single(x => x.SubmissionId.Equals(4)).Balance);
        }

        [Test]
        public async Task ThenTransactionAreaggregatedCorrectyForMultiple()
        {
            //Arrange
            var transactions = new List<TransactionLine>
            {
                new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 1,
                    TransactionDate = DateTime.Now.AddDays(-120),
                    Amount = 1000,
                    TransactionType = LevyItemType.TopUp,
                    EmpRef = "123"
                },
                new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 2,
                    TransactionDate = DateTime.Now.AddDays(-100),
                    Amount = 1000,
                    TransactionType = LevyItemType.TopUp,
                    EmpRef = "123"
                },
                new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 3,
                    TransactionDate = DateTime.Now.AddDays(-80),
                    Amount = -500,
                    TransactionType = LevyItemType.Declaration,
                    EmpRef = "123"
                },
                new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 4,
                    TransactionDate = DateTime.Now.AddDays(-50),
                    Amount = 2000,
                    TransactionType = LevyItemType.Declaration,
                    EmpRef = "123"
                },
                 new TransactionLine
                {
                    AccountId = 1,
                    SubmissionId = 5,
                    TransactionDate = DateTime.Now.AddDays(-10),
                    Amount = 2000,
                    TransactionType = LevyItemType.Declaration,
                    EmpRef = "123"
                }
            };

            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAccountTransactionsQuery>())).ReturnsAsync(new ValidationResult());
            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(transactions);

            _accountHistoryList.Add(new AccountHistoryEntry
            {
                AccountId = 1,
                DateAdded = DateTime.Now.AddDays(-90),
                DateRemoved = DateTime.Now.AddDays(-60),
                PayeRef = "123"
            });

            _accountHistoryList.Add(new AccountHistoryEntry
            {
                AccountId = 1,
                DateAdded = DateTime.Now.AddDays(-11),
                PayeRef = "123"
            });

            //Act
            var response = await RequestHandler.Handle(new GetEmployerAccountTransactionsQuery
            {
                AccountId = 1,
                ExternalUserId = "3EFR",
                HashedId = "123"
            });

            //Assert
            Assert.AreEqual(4, response.Data.TransactionLines.Count);
            Assert.AreEqual(3500, response.Data.TransactionLines.First(x => x.SubmissionId.Equals(0)).Balance);
            Assert.AreEqual(2000, response.Data.TransactionLines.Last(x => x.SubmissionId.Equals(0)).Balance);
            Assert.AreEqual(1500, response.Data.TransactionLines.Single(x => x.SubmissionId.Equals(3)).Balance);
            Assert.AreEqual(5500, response.Data.TransactionLines.Single(x => x.SubmissionId.Equals(5)).Balance);
        }
    }
}
