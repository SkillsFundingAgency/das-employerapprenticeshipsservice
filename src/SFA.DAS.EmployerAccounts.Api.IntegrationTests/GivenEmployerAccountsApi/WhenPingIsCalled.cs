using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi;

[ExcludeFromCodeCoverage]
[TestFixture]
public class WhenPingIsCalled : GivenEmployerAccountsApi
{

    [SetUp]
    public void SetUp()
    {
        WhenControllerActionIsCalled("/ping");
    }

    [Test]
    public void ThenOkResponseIsReturn()
    {
        Response?
            .StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }
}