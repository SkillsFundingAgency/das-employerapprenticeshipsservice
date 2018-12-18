using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class AccountLegalEntitesApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accountlegalentities";
    }
}