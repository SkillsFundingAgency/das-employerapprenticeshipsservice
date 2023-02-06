using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
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
        WhenControllerActionIsCalled($"https://localhost:44330/api/accounts/MADE*UP*ID");
    }

    [Test]
    public async Task ThenTheStatusShouldBeNotFound_ByHashedId()
    {
        Response.ExpectStatusCodes(HttpStatusCode.NotFound);
    }


}