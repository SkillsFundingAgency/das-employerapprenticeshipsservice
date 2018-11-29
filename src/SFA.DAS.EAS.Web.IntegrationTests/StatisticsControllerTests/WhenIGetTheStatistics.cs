using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.StatisticsControllerTests
{
    [TestFixture]
    public class WhenIGetTheStatistics
    {
        [Test]
        public async Task ThenTheStatusShouldBeOk()
        {
            var call = new CallRequirements("api/statistics")
                .AllowStatusCodes(HttpStatusCode.OK);

            var response = await ApiIntegrationTester.InvokeIsolatedGetAsync(call);

            // Assert
            Assert.That(response.Response.StatusCode == HttpStatusCode.OK);
        }
    }
}
