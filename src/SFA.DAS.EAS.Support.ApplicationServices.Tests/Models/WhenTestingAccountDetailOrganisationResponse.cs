using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.Models;

[TestFixture]
public class WhenTestingAccountDetailOrganisationResponse
{
    [Test]
    public void ItShouldDefaultToNotFoundStatusCode()
    {
        Assert.That(new AccountDetailOrganisationsResponse().StatusCode, Is.EqualTo(SearchResponseCodes.NoSearchResultsFound));
    }

    [Test]
    public void ItShouldDefaultToNullAccount()
    {
        Assert.That(new AccountDetailOrganisationsResponse().Account, Is.Null);
    }
}