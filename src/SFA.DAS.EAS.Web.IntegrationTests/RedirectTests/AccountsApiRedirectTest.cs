using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class UserAccountApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/user/userRef/accounts";
    }
}