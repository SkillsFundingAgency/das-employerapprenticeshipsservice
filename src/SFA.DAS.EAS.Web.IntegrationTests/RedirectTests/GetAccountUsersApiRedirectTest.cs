using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class GetAccountUsersApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/HashedAccountId/users";
    }
}