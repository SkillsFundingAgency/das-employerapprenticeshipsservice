using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper;
using SFA.DAS.EAS.Account.Api.Controllers;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGetAccounts
    {
        private ApiIntegrationTester _tester;

        [SetUp]
        public void SetUp()
        {
            _tester = new ApiIntegrationTester();
        }

        [TearDown]
        public void TearDown()
        {
            _tester.Dispose();
        }

        [Test]
        public async Task ThenTheStatusShouldBeFound_AndWeShouldHaveAtLeastTheKnownAccount()
        {
            // Arrange
            const string accountName = "ACME Fireworks";
            const string legalEntityName = "RoadRunner Pest Control";
            const string payeReference = "Acme PAYE";

            const string accountName2 = "ACME Fireworks2";
            const string legalEntityName2 = "RoadRunner Pest Control2";
            const string payeReference2 = "Acme PAYE2";

            var builder = _tester.DbBuilder;

            CreateTestAccount(builder, accountName, payeReference, legalEntityName);
            CreateTestAccount(builder, accountName2, payeReference2, legalEntityName2);

            var callRequirements = new CallRequirements($"api/accounts/")
                .ExpectControllerType(typeof(EmployerAccountsController))
                .AllowStatusCodes(HttpStatusCode.OK);
            
            // Act
            var accounts = await _tester.InvokeGetAsync<PagedApiResponseViewModel<AccountWithBalanceViewModel>>(callRequirements);

            // Assert
            Assert.IsTrue(accounts.Data.Data.Any(a => a.AccountName == accountName));
            Assert.IsTrue(accounts.Data.Data.Any(a => a.AccountName == accountName2));
        }

        private static void CreateTestAccount(DbBuilder builder, string accountName, string payeReference, string legalEntityName)
        {
            builder
                .BeginTransaction()
                .EnsureUserExists(builder.BuildUserInput())
                .EnsureAccountExists(builder.BuildEmployerAccountInput(accountName, payeReference))
                .WithLegalEntity(builder.BuildEntityWithAgreementInput(legalEntityName))
                .CommitTransaction();
        }
    }
}