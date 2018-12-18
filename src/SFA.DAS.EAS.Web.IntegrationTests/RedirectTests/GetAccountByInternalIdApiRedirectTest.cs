using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class GetAccountByInternalIdApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/internal/-1";
    }
}