using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ModelBuilders;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Service.EmployerAccounts
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
            _tester = new ApiIntegrationTester(TestSetupIoC.CreateIoC);

            // Arrange
            const string accountName = "ACME Fireworks";
            const string legalEntityName = "RoadRunner Pest Control";
            const string payeReference = "Acme PAYE";

            _tester.InitialiseData<EmployerAccountsDbBuilder>(builder =>
            {
                // TODO: the way ids are propagated is a bit clunky
                builder
                    .EnsureUserExists(TestModelBuilder.User.CreateUserInput())
                    .EnsureAccountExists(TestModelBuilder.Account.CreateAccountInput(accountName, payeReference, builder.Context.ActiveUser.UserId))
                    .WithLegalEntity(TestModelBuilder.LegalEntity.BuildEntityWithAgreementInput(legalEntityName, builder.Context.ActiveEmployerAccount.AccountId));

                _hashedAccountId = builder.Context.ActiveEmployerAccount.HashedAccountId;
                _accountId = builder.Context.ActiveEmployerAccount.AccountId;
            });
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