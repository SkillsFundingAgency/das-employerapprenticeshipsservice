using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Application.Validation;
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
            Assert.AreEqual(1,response.Data.TransactionLines.Count);
        }
    }
}
