using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class GetAccountUsersByInternalAccountIdApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/internal/-1/users";
    }
}