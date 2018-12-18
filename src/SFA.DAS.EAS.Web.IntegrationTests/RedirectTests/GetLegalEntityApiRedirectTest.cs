using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class GetLegalEntityApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/G6M7RV/legalentities/4";
    }
}