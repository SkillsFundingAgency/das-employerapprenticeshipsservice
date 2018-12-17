using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ApiTester;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Service.EmployerAccounts
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