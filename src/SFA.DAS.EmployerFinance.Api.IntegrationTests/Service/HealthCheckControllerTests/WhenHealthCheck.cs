using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Api.IntegrationTests.TestUtils.ApiTester;

namespace SFA.DAS.EmployerFinance.Api.IntegrationTests.Service.HealthCheckControllerTests
{

    [TestFixture]
    public class WhenGettingHealthCheck
    {
        [Test]
        public async Task ThenTheStatusShouldBeOk()
        {
            // Arrange
            var call = new CallRequirements("api/HealthCheck");

            // Act
            var response = await ApiIntegrationTester.InvokeIsolatedGetAsync(call);

            // Assert

            response.ExpectStatusCodes(HttpStatusCode.OK);
            Assert.Pass("Verified we got OK");
        }
    }
}
