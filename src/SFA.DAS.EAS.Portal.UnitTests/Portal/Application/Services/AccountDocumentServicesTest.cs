using System;
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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions.Common;
using Microsoft.ApplicationInsights.Metrics.Extensibility;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Services
{
    [Parallelizable]
    [TestFixture]
    class AccountDocumentServicesTest
    {
        public class TestContext
        {
            public readonly AccountDocument TestAccountDocument;
            public long AccountId = 1;
            public readonly AccountDocumentService AccountDocumentService;
            private Mock<IAccountsRepository> _accountRepoMock;

            public TestContext()
            {
                _accountRepoMock = new Mock<IAccountsRepository>();

                Account testAccount = new AccountBuilder().WithOrganisation(new OrganisationBuilder().WithId(AccountId).WithCohort(new CohortBuilder().WithId("Cohort")));
                TestAccountDocument = new AccountDocument
                {
                    Account = testAccount
                };

                _accountRepoMock.Setup(x => x.GetAccountDocumentById(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(TestAccountDocument);
                AccountDocumentService = new AccountDocumentService(_accountRepoMock.Object);
            }
        }

        [Test]
        public async Task WhenAccountServiceIsCalledWithAStoredAccountIdGetTheAccount()
        {
            //Arrange
            var testContext = new TestContext();

            //Act
            var result = await testContext.AccountDocumentService.GetOrCreate(testContext.AccountId, It.IsAny<CancellationToken>());

            //Assert
            Assert.AreSame(result, testContext.TestAccountDocument);
            Assert.IsFalse(result.IsNew);
        }


       [Test]
        public async Task WhenAccountServiceIsCalledWithANewAccountIdCreateTheAccount()
        {
            //Arrange
            var testContext = new TestContext();
            var expected = new AccountDocument
            {
                Account = new Account
                {
                    Id = 2
                }
            };

            //Act
            var result = await testContext.AccountDocumentService.GetOrCreate(2, It.IsAny<CancellationToken>());

            //Assert
            Assert.AreEqual(expected.AccountId, result.AccountId);
            result.Account.IsSameOrEqualTo(expected.Account);
            Assert.IsTrue(result.IsNew);
        }
    }
}
