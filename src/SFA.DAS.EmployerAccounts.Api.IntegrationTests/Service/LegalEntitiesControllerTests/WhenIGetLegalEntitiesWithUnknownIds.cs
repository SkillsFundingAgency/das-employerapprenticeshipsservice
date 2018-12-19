using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Service.LegalEntitiesControllerTests
{
    [TestFixture]

    public class WhenIGetLegalEntitiesWithUnknownIds
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
        public async Task ThenTheStatusShouldBeNotFound_ByHashedId()
        {
            // Arrange
            var callRequirements = new CallRequirements("api/accounts/MADE*UP*ID/legalentities");

            // Act
            var returnResponse = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            returnResponse.ExpectStatusCodes(HttpStatusCode.NotFound);
            Assert.Pass("Verified we got http status NotFound");
        }
    }
}