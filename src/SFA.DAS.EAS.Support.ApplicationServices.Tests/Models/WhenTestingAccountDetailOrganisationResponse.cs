using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.Models
{
    [TestFixture]
    public class WhenTestingAccountDetailOrganisationResponse
    {
        [Test]
        public void ItShouldDefaultToNotFoundStatusCode()
        {
            Assert.AreEqual(SearchResponseCodes.NoSearchResultsFound,
                new AccountDetailOrganisationsResponse().StatusCode);
        }

        [Test]
        public void ItShouldDefaultToNullAccount()
        {
            Assert.IsNull(new AccountDetailOrganisationsResponse().Account);
        }
    }
}