using System.Diagnostics.CodeAnalysis;
using System.Net;
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
        WhenControllerActionIsCalled(@"/api/accounts/%20/legalentities");
    }

    [Test]
    public void ThenTheStatusShouldBeBadRequest()
    {
        Response.ExpectStatusCodes(HttpStatusCode.BadRequest);
    }
}