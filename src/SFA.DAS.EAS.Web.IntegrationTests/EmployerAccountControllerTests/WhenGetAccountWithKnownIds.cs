using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess;
using SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGetAccountWithKnownIds
    {
        private ApiIntegrationTester _tester;
        private EmployerAccountOutput _employerAccount;

        [SetUp]
        public async Task SetUp()
        {
            _tester = new ApiIntegrationTester(TestSetupIoC.CreateIoC);

            // Arrange
            await _tester.InitialiseData<EmployerAccountsDbBuilder>(async builder =>
            {
                var data = new TestModelBuilder()
                        .WithNewUser()
                        .WithNewAccount()
                        .WithNewLegalEntity();

                await builder.SetupDataAsync(data);

                _employerAccount = data.CurrentAccount.AccountOutput;
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

            var callRequirements = new CallRequirements($"api/accounts/{_employerAccount.HashedAccountId}");
            
            // Act
            var account = await _tester.InvokeGetAsync<AccountDetailViewModel>(callRequirements);

            // Assert
            account.ExpectControllerType(typeof(EmployerAccountsController));
            account.ExpectStatusCodes(HttpStatusCode.OK);
            Assert.IsNotNull(account.Data);
        }


        [Test]
        public async Task ThenTheStatusShouldBeFound_ByAccountId()
        {
            var callRequirements = new CallRequirements($"api/accounts/internal/{_employerAccount.AccountId}");

            // Act
            var account = await _tester.InvokeGetAsync<AccountDetailViewModel>(callRequirements);

            // Assert
            account.ExpectControllerType(typeof(EmployerAccountsController));
            account.ExpectStatusCodes(HttpStatusCode.OK);
            Assert.IsNotNull(account.Data);
        }
    }
}