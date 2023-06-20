using System.Net;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi.EmployerAccountControllerTests;

[TestFixture]
public class WhenGetAccountUsersWithUnknownIds : GivenEmployerAccountsApi
{
    [SetUp]
    public void SetUp()
    {
        WhenControllerActionIsCalled("/api/accounts/MADE*UP*ID/users");
    }

    [Test]
    public void ThenTheStatusShouldBeOK_AndDataShouldContainZeroUsers()
    {
        Response?.ExpectStatusCodes(HttpStatusCode.OK);
        Assert.AreEqual(0, Response?.GetContent<List<TeamMember>>().Count);

        Assert.Pass("Verified we got http status OK");
    }
}