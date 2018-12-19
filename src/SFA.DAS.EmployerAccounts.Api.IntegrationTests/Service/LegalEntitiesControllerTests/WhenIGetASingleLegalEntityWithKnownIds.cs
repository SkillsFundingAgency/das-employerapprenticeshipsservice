using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess.Dtos;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ModelBuilders;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Service.LegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetASingleLegalEntityWithKnownIds
    {
        private ApiIntegrationTester _tester;
        private EmployerAccountOutput _employerAccount;

        [SetUp]
        public async Task Setup()
        {
            _tester = new ApiIntegrationTester(TestSetupIoC.CreateIoC);

            // Arrange
            await _tester.InitialiseData<EmployerAccountsDbBuilder>(async builder =>
            {
                var data = new TestModelBuilder()
                    .WithNewUser()
                    .WithNewAccount();

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
            var callRequirements = new CallRequirements($"api/accounts/{_employerAccount.HashedAccountId}/legalentities");

            // Act
            var account = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            Assert.IsNotNull(account.Data);
            Assert.AreEqual(1, account.Data.Count);
        }
    }
}