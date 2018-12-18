using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class AccountApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/HashedAccountId";
    }
}