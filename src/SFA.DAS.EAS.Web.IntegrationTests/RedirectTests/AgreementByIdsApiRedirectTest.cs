using NUnit.Framework;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    [TestFixture]
    public class AgreementByIdApiRedirectTest : ApiRedirectTest
    {
        protected override string PathAndQuery => "/api/accounts/hashedAccountId/legalEntities/hashedlegalEntityId/agreements/agreementid";
    }
}