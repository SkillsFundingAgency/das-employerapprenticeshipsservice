using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.StatisitcsControllerTests
{
    [TestFixture]
    public class WhenIGetTheStatistics
    {
        [Test]
        public async Task ThenTheStatusShouldBeOk()
        {
            var call = new CallRequirements("api/statistics")
                .AllowStatusCodes(HttpStatusCode.OK);

            var apiTestResolver = new ApiIntegrationTester();
            var item = apiTestResolver.Resolve<IStatisticsAccountsRepository>();



            await ApiIntegrationTester.InvokeIsolatedGetAsync(call);

            // Assert
            Assert.Pass("Verified we got OK");
        }
    }
}
