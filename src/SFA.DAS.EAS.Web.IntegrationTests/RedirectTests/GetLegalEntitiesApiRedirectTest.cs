using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class GetLegalEntitiesApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/hashedAccountId/legalentities";
    }
}