using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.HealthCheckControllerTests
{

    [TestFixture]
    public class WhenGettingHealthCheck
    {
        [Test]
        public async Task ThenTheStatusShouldBeOk()
        {
            // Arrange
            var call = new CallRequirements("api/HealthCheck")
                            .AllowStatusCodes(HttpStatusCode.OK);

            // Act
            await ApiIntegrationTester.InvokeIsolatedGetAsync(call);

            // Assert
            Assert.Pass("Verified we got OK");
        }
    }
}
