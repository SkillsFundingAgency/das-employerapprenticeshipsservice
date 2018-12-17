using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests;
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
            var call = new CallRequirements("api/HealthCheck");

            // Act
            var response = await ApiIntegrationTester.InvokeIsolatedGetAsync(call);

            // Assert

            response.ExpectStatusCodes(HttpStatusCode.OK);
            Assert.Pass("Verified we got OK");
        }
    }
}
