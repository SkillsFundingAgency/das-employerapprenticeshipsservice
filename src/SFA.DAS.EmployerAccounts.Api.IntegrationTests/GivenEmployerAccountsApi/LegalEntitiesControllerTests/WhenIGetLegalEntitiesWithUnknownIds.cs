using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi.LegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetLegalEntitiesWithUnknownIds
    :GivenEmployerAccountsApi
    {
        [SetUp]
        public void Setup()
        {
            WhenControllerActionIsCalled($"https://localhost:44330/api/accounts/MADE*UP*ID/legalentities");
        }
        [Test]
        public async Task ThenTheStatusShouldBeNotFound_ByHashedId()
        {
            Response.ExpectStatusCodes(HttpStatusCode.NotFound);
            Assert.Pass("Verified we got http status NotFound");
        }
    }
}