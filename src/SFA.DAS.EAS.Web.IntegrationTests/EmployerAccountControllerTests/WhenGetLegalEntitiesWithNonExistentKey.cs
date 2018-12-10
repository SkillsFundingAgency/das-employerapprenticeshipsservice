using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess;
using SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGetLegalEntitiesWithNonExistentKey
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
        public async Task ThenTheStatusShouldBeNotFound()
        {
            // Arrange
            var callRequirements =
                new CallRequirements("api/accounts/ZZZZZZ/legalentities")
                    .AllowStatusCodes(HttpStatusCode.NotFound)
                    .ExpectControllerType(typeof(LegalEntitiesController));

            // Act
            var legalEntities = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            Assert.IsNull(legalEntities.Data);
        }

        [Test]
        public async Task ThenTheStatusShouldBeFound()
        {
            // Arrange
            const string accountName = "ACME Fireworks";
            const string legalEntityName = "RoadRunner Pest Control";
            const string payeReference = "Acme PAYE";

            string hashedAccountId;
            using (var testEmployerAccountsDbBuilder = _tester.GetTransientInstance<EmployerAccountsDbBuilder>())
            {
                var userInput = TestModelBuilder.User.CreateUserInput();

                testEmployerAccountsDbBuilder
                    .EnsureUserExists(userInput)
                    .EnsureAccountExists(TestModelBuilder.Account.CreateAccountInput(accountName, payeReference, testEmployerAccountsDbBuilder.Context.ActiveUser.UserId))
                    .WithLegalEntity(TestModelBuilder.LegalEntity.BuildEntityWithAgreementInput(legalEntityName, testEmployerAccountsDbBuilder.Context.ActiveEmployerAccount.AccountId));

                hashedAccountId = testEmployerAccountsDbBuilder.Context.ActiveEmployerAccount.HashedAccountId;
            }

            var callRequirements =
                new CallRequirements($"api/accounts/{hashedAccountId}/legalentities")
                    .AllowStatusCodes(HttpStatusCode.OK)
                    .ExpectControllerType(typeof(LegalEntitiesController));

            // Act
            var legalEntities = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            Assert.IsNotNull(legalEntities.Data);
        }
    }
}