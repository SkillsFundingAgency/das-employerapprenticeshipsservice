using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi.LegalEntitiesControllerTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class WhenGetLegalEntitiesWithNoKey : GivenEmployerAccountsApi
{
    [SetUp]
    public void Setup()
    {
        WhenControllerActionIsCalled(@"https://localhost:44330/api/accounts/%20/legalentities");
    }
    [Test]
    public async Task ThenTheStatusShouldBeBadRequest()
    {
        Response.ExpectStatusCodes(HttpStatusCode.BadRequest);
    }
}