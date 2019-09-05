using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class WhenHealthCheckIsCalled : GivenEmployerAccountsApi
    {
        protected override string GetRequestUri()
        {
            return @"https://localhost:44330/api/healthcheck";
        }

        [Test]
        public void ThenOkResponseIsReturn()
        {
            Response
                .StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }
    }
}