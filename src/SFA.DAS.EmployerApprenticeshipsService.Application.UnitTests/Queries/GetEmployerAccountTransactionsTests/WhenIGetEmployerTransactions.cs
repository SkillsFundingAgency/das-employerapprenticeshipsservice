using System;
using System.Collections.Generic;
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
        private GetEmployerAccountTransactionsQuery _request;
        private Mock<IApprenticeshipInfoServiceWrapper> _apprenticshipInfoService;

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
                ExternalUserId = "3EFR",
                HashedId = "RTF34"
            };

            _dasLevyService = new Mock<IDasLevyService>();
            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>()))
                           .ReturnsAsync(new List<TransactionLine>());

            _apprenticshipInfoService = new Mock<IApprenticeshipInfoServiceWrapper>();

            RequestHandler = new GetEmployerAccountTransactionsHandler(_dasLevyService.Object, RequestValidator.Object, _apprenticshipInfoService.Object);
            Query = new GetEmployerAccountTransactionsQuery();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(_request);

            //Assert
            _dasLevyService.Verify(x => x.GetTransactionsByAccountId(_request.AccountId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var transactions = new List<TransactionLine>
                {
                    new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 1,
                        TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 1000,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    }
                };

            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>()))
                           .ReturnsAsync(transactions);

            //Act
            var response = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual(_request.HashedId, response.Data.HashedId);
            Assert.AreEqual(_request.AccountId, response.Data.AccountId);
            Assert.AreEqual(1, response.Data.TransactionLines.Count);
        }
        

        [Test]
        public async Task ThenIfNoTransactionAreFoundAnEmptyTransactionListIsReturned()
        {
            //Act
            var response = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual(_request.HashedId, response.Data.HashedId);
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
                    new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 1,
                        TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 1000,
                        TransactionType = LevyItemType.Payment,
                        EmpRef = "123",
                        UkPrn = expectedUkprn
                    }
                };
            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>())).ReturnsAsync(transactions);
            _apprenticshipInfoService.Setup(x => x.GetProvider(expectedUkprn)).Returns(new ProvidersView {Providers = new List<Provider> {new Provider {ProviderName = "test"}}});

            //Act
            await RequestHandler.Handle(_request);

            //Act
            _apprenticshipInfoService.Verify(x=>x.GetProvider(expectedUkprn),Times.Once);
        }
    }
}
