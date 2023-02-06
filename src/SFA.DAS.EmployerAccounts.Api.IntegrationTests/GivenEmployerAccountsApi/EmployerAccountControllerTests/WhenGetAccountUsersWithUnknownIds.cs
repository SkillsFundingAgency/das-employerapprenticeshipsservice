using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi.EmployerAccountControllerTests;

[TestFixture]
public class WhenGetAccountUsersWithUnknownIds :GivenEmployerAccountsApi
{
    [SetUp]
    public void SetUp()
    {
        WhenControllerActionIsCalled($"https://localhost:44330/api/accounts/MADE*UP*ID/users");
    }

    [Test]
    public async Task ThenTheStatusShouldBeOK_AndDataShouldContainZeroUsers()
    {
        Response.ExpectStatusCodes(HttpStatusCode.OK);
        Assert.AreEqual(0, Response.GetContent<List<TeamMember>>().Count);

        Assert.Pass("Verified we got http status OK");
    }

       
}