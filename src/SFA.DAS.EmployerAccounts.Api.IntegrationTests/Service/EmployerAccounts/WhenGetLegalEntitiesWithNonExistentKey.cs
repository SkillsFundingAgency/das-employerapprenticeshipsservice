using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ModelBuilders;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Service.EmployerAccounts
{
    [TestFixture]
    public class WhenGetLegalEntitiesWithNonExistentKey
    {
        private ApiIntegrationTester _tester;

        [SetUp]
        public void SetUp()
        {
            _tester = new ApiIntegrationTester(TestSetupIoC.CreateIoC);
        }

        [TearDown]
        public void TearDown()
        {
            _tester.Dispose();
        }

        //[Test]
        public async Task ThenTheStatusShouldBeNotFound()
        {
            // Arrange
            var callRequirements =
                new CallRequirements("api/accounts/ZZZZZZ/legalentities");

            // Act
            var legalEntities = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert

            legalEntities.ExpectStatusCodes(HttpStatusCode.NotFound);
            legalEntities.ExpectControllerType(typeof(LegalEntitiesController));
            Assert.IsNull(legalEntities.Data);
        }

        //[Test]
        public async Task ThenTheStatusShouldBeFound()
        {
            // Arrange
            string hashedAccountId = null;
            await _tester.InitialiseData<EmployerAccountsDbBuilder>(async builder =>
            {
                var data = new TestModelBuilder()
                    .WithNewUser()
                    .WithNewAccount()
                    .WithNewLegalEntity();

                await builder.SetupDataAsync(data);

                hashedAccountId = data.CurrentAccount.AccountOutput.HashedAccountId;
            });

            var callRequirements =
                new CallRequirements($"api/accounts/{hashedAccountId}/legalentities");

            // Act
            var legalEntities = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            legalEntities.ExpectStatusCodes(HttpStatusCode.OK);
            legalEntities.ExpectControllerType(typeof(LegalEntitiesController));
            Assert.IsNotNull(legalEntities.Data);
        }
    }
}