using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.Owin.Security.DataHandler.Encoder;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Api.Controllers;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

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
                    .ExpectControllerType(typeof(AccountLegalEntitiesController))
                    .ExpectValidationError();

            // Act
            await ApiIntegrationTester.InvokeIsolatedGetAsync(callRequirements);

            // Assert
            Assert.Pass("Verfiied we got a bad request type behaviour");
        }
    }
}
