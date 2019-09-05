using System.Net;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi.LegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetLegalEntitiesWithUnknownIds
    :GivenEmployerAccountsApi
    {
        [Test]
        public void ThenNotFoundResponseIsReturned()
        {
            Response
                .StatusCode
                .Should()
                .Be(HttpStatusCode.NotFound);
        }

        protected override string GetRequestUri()
        {
            return @"https://localhost:44330/api/accounts/MADE*UP*ID/legalentities";
        }
    }
}