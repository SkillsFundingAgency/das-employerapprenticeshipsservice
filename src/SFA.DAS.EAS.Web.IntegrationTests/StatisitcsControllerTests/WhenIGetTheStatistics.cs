using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.StatisitcsControllerTests
{
    [TestFixture]
    public class WhenIGetTheStatistics
    {
        [Test]
        public async Task ThenTheStatusShouldBeOk()
        {
            var call = new CallRequirements("api/statistics");

            var response = await ApiIntegrationTester.InvokeIsolatedGetAsync(call);

            // Assert

            response.ExpectStatusCodes(HttpStatusCode.OK);
            Assert.Pass("Verified we got OK");
        }
    }
}
