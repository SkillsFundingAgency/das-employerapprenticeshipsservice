using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class GetTransactionsApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/hashedAccountId/transactions/2018/12";
    }
}