using System.Diagnostics.CodeAnalysis;
using System.Net;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi.EmployerAccountControllerTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class WhenGetAccountWithUnknownIds : GivenEmployerAccountsApi
{
    [SetUp]
    public void SetUp()
    {
        WhenControllerActionIsCalled("/api/accounts/MADE*UP*ID");
    }

    [Test]
    public void ThenTheStatusShouldBeNotFound_ByHashedId()
    {
        Response.ExpectStatusCodes(HttpStatusCode.NotFound);
    }
}