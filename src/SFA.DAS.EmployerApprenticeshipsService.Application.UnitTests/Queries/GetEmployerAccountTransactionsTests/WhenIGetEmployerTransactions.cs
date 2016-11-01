using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountTransactionsTests
{
    public class WhenIGetEmployerTransactions : QueryBaseTest<GetEmployerAccountTransactionsHandler, GetEmployerAccountTransactionsQuery, GetEmployerAccountTransactionsResponse>
    {
        private Mock<IDasLevyService> _dasLevyService;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _dasLevyService = new Mock<IDasLevyService>();
            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(new List<TransactionLine> {new TransactionLine()});

            RequestHandler = new GetEmployerAccountTransactionsHandler(_dasLevyService.Object, RequestValidator.Object);
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
            Assert.AreEqual(1,response.Data.TransactionLines.Count);
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
    }
}
