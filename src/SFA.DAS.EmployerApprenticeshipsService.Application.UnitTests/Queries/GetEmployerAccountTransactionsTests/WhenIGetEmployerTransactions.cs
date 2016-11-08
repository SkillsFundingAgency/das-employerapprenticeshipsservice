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
        private Mock<IDasLevyService> _dasLevyRepository;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _dasLevyRepository = new Mock<IDasLevyService>();
            _dasLevyRepository.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(new List<TransactionLine> {new TransactionLine()});

            RequestHandler = new GetEmployerAccountTransactionsHandler(_dasLevyRepository.Object, RequestValidator.Object);
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
            _dasLevyRepository.Verify(x=>x.GetTransactionsByAccountId(expectedAcountId));
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
    }
}
