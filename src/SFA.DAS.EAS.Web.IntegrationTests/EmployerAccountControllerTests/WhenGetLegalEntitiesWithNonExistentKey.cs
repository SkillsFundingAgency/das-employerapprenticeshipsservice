using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils;
using SFA.DAS.EAS.Api.Controllers;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{

    [TestFixture]
    public class WhenGetLegalEntitiesWithNonExistentKey
    {
        [Test]
        public async Task ThenTheStatusShouldBeNotFound()
        {
            // Arrange
            var callRequirements =
                new CallRequirements<LegalEntities>("api/accounts/ZZZZZZ/legalentities", HttpStatusCode.NotFound)
                {
                    ExpectedControllerType = typeof(AccountLegalEntitiesController)
                };

            // Act
            var legalEntities = await ApiIntegrationTester.InvokeIsolatedGetAsync(callRequirements);

            // Assert
            Assert.IsNull(legalEntities);
        }
    }
}
