using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class GetAccountApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/HashedAccountId";
    }
}