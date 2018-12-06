using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.API.IntegrationTests.Extensions;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGetAccountWithKnownIds
    {
        private ApiIntegrationTester _tester;
        private string _hashedAccountId;
        private long _accountId;

        [SetUp]
        public void SetUp()
        {
            _tester = new ApiIntegrationTester();

            // Arrange
            const string accountName = "ACME Fireworks";
            const string legalEntityName = "RoadRunner Pest Control";
            const string payeReference = "Acme PAYE";

            using (var testDbContext = _tester.GetTransientInstance<EmployerAccountsDbBuilder>())
            {
                testDbContext
                    .EnsureUserExists(testDbContext.BuildUserInput())
                    .EnsureAccountExists(testDbContext.BuildEmployerAccountInput(accountName, payeReference))
                    .WithLegalEntity(testDbContext.BuildEntityWithAgreementInput(legalEntityName));

                _hashedAccountId = testDbContext.Context.ActiveEmployerAccount.HashedAccountId;
                _accountId = testDbContext.Context.ActiveEmployerAccount.AccountId;
            }
        }

        [TearDown]
        public void TearDown()
        {
            _tester.Dispose();
        }

        [Test]
        public async Task ThenTheStatusShouldBeFound_ByHashedAccountId()
        {

            var callRequirements = new CallRequirements($"api/accounts/{_hashedAccountId}")
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
            var callRequirements = new CallRequirements($"api/accounts/internal/{_accountId}")
                .ExpectControllerType(typeof(EmployerAccountsController))
                .AllowStatusCodes(HttpStatusCode.OK);

            // Act
            var account = await _tester.InvokeGetAsync<AccountDetailViewModel>(callRequirements);

            // Assert
            Assert.IsNotNull(account.Data);
        }
    }
}