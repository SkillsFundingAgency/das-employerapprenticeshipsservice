using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.Database;
using SFA.DAS.EAS.Portal.UnitTests.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Services
{
    [Parallelizable]
    [TestFixture]
    class AccountDocumentServicesTest
    {
        public class TestContext
        {
            public AccountDocument TestAccountDocument { get; set; }
            public Account TestAccount { get; set; }
            public long AccountId = 1;
            public AccountDocumentService AccountDocumentService { get; set; }
            Mock<IAccountsRepository> MockAccountsRepository { get; set; }
            Mock<IDocumentRepository<AccountDocument>> documentRepoMock;

            public TestContext()
            {
                TestAccountDocument = new AccountDocument
                {
                    Account = TestAccount
                };
                documentRepoMock.Setup(mock => mock.CreateQuery(null)).Returns(new List<AccountDocument>
                {
                    TestAccountDocument
                }.AsQueryable());
                MockAccountsRepository = new Mock<IAccountsRepository>();
                TestAccount = new AccountBuilder().WithOrganisation(new OrganisationBuilder().WithId(AccountId).WithCohort(new CohortBuilder().WithId("Cohort")));
                
                MockAccountsRepository.Setup(m => m.CreateQuery(null)).Returns(new List<AccountDocument>
                {
                    TestAccountDocument
                }.AsQueryable().AsDocumentQuery());

                AccountDocumentService = new AccountDocumentService(MockAccountsRepository.Object);
            }
        }

        [Test]
        public async Task WhenAccountServiceIsCalledWithAStoredAccountIdGetTheAccount()
        {
            //Arrange
            var TestContext = new TestContext();
            var AccountId = 1;

            //Act
            var result = await TestContext.AccountDocumentService.GetOrCreate(AccountId, It.IsAny<CancellationToken>());

            //Assert
            Assert.AreSame(result, TestContext.TestAccountDocument);
        }


       /* [Test]
        public async Task WhenAccountServiceIsCalledWithANewAccountIdCreateTheAccount()
        {

        }*/
    }
}
