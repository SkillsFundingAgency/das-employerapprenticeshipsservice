using System.Net;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi.LegalEntitiesControllerTests;

[TestFixture]
public class WhenIGetLegalEntitiesWithUnknownIds :GivenEmployerAccountsApi
{
    [SetUp]
    public void Setup()
    {
        WhenControllerActionIsCalled("/api/accounts/MADE*UP*ID/legalentities");
    }

    [Test]
    public void ThenTheStatusShouldBeNotFound_ByHashedId()
    {
        Response.ExpectStatusCodes(HttpStatusCode.NotFound);
        Assert.Pass("Verified we got http status NotFound");
    }

}