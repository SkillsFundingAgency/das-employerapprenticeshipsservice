using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class GetTransferConnectionsApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/G6M7RV/transfers/connections";
    }
}