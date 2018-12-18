using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class StatisticsApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/statistics";
    }
}