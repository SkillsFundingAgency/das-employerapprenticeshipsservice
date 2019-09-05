using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.DataHelpers;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi.LegalEntitiesControllerTests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class WhenIGetASingleLegalEntityWithKnownIds
        : GivenEmployerAccountsApi
    {
        [Test]
        public void ThenCorrectResourceListShouldBeReturned()
        {
            Response
                .ShouldHaveContentOfType<ResourceList>();

            var content = Response.GetContent<ResourceList>();

            content
                .Count
                .Should()
                .Be(1);

            var firstResource = content[0];

            firstResource
                .Id
                .Should()
                .Be("3");

            firstResource
                .Href
                .Should()
                .Be(@"/api/accounts/JLVKPM/legalentities/3");
        }

        [Test]
        public void ThenOkResponseIsReturn()
        {
            Response
                .StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }


        protected override string GetRequestUri()
        {
            return @"https://localhost:44330/api/accounts/JLVKPM/legalentities";
        }
    }
}