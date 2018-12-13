using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.Api.Controllers;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGetLegalEntitiesWithNoKey
    {
        [Test]
        public async Task ThenTheStatusShouldBeBadRequest()
        {
            // Arrange
            var callRequirements =
                new CallRequirements("api/accounts/%20/legalentities")
                    .ExpectControllerType(typeof(LegalEntitiesController))
                    .ExpectValidationError();

            // Act
            await ApiIntegrationTester.InvokeIsolatedGetAsync(callRequirements);

            // Assert
            Assert.Pass("Verified we got a bad request type behaviour");
        }
    }
}