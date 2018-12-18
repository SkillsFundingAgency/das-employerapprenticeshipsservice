using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class GetTransactionSummaryApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/hashedAccountId/transactions";
    }
}