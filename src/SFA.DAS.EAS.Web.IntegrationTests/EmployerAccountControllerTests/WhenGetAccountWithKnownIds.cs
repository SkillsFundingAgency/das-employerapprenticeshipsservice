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
    public class WhenGetAccountWithKnownIds
    {
        private ApiIntegrationTester _tester;

        [SetUp]
        public void SetUp()
        {
            _tester = new ApiIntegrationTester();

            // Arrange
            const string accountName = "ACME Fireworks";
            const string legalEntityName = "RoadRunner Pest Control";

            var builder = _tester.DbBuilder;
            builder
                .BeginTransaction()
                .EnsureUserExists(builder.BuildUserInput())
                .EnsureAccountExists(builder.BuildEmployerAccountInput(accountName))
                .WithLegalEntity(builder.BuildEntityWithAgreementInput(legalEntityName))
                .CommitTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            _tester.Dispose();
        }

        [Test]
        public async Task ThenTheStatusShouldBeFound_ByHashedAccountId()
        {
            var hashedAccountId = _tester.DbBuilder.Context.ActiveEmployerAccount.HashedAccountId;

            var callRequirements = new CallRequirements($"api/accounts/{hashedAccountId}")
                .ExpectControllerType(typeof(EmployerAccountsController))
                .AllowStatusCodes(HttpStatusCode.OK);
            
            // Act
            var account = await _tester.InvokeGetAsync<AccountDetailViewModel>(callRequirements);

            // Assert
            Assert.IsNotNull(account.Data);
        }


        [Test]
        public async Task ThenTheStatusShouldBeFound_ByAccountId()
        {
            var accountId = _tester.DbBuilder.Context.ActiveEmployerAccount.AccountId;

            var callRequirements = new CallRequirements($"api/accounts/internal/{accountId}")
                .ExpectControllerType(typeof(EmployerAccountsController))
                .AllowStatusCodes(HttpStatusCode.OK);

            // Act
            var account = await _tester.InvokeGetAsync<AccountDetailViewModel>(callRequirements);

            // Assert
            Assert.IsNotNull(account.Data);
        }
    }
}