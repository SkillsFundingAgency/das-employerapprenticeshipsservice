using System.Collections;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.LegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetLegalEntitiesWithKnownIds
    {
        private ApiIntegrationTester _tester;

        [SetUp]
        public void Setup()
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

            var callRequirements = new CallRequirements($"api/accounts/{hashedAccountId}/legalentities")
                .ExpectControllerType(typeof(LegalEntitiesController))
                .AllowStatusCodes(HttpStatusCode.OK);

            // Act
            var account = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            Assert.IsNotNull(account.Data);
 //           account.Data[0];

        }
    }
}