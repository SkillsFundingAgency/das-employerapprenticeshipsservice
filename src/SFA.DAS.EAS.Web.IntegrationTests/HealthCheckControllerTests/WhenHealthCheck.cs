using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.HealthCheckControllerTests
{

    [TestFixture]
    public class WhenGettingHealthCheck
    {
        [Test]
        public async Task ThenTheStatusShouldBeOk()
        {
            await ApiIntegrationTester.InvokeIsolatedGetAsync(
                new CallRequirements("api/HealthCheck", HttpStatusCode.OK));

            Assert.Pass("Verified we got OK");
        }
    }
}
