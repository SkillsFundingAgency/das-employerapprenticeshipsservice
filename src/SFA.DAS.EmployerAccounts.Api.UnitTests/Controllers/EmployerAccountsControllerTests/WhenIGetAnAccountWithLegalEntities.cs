using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.EmployerAccounts.TestCommon.Extensions;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public class WhenIGetAnAccountWithLegalEntities : EmployerAccountsControllerTests
    {
        [Test]
        public async Task ThenTheAccountIsReturnedAndTheUriIsCorrect()
        {
            var hashedAccountId = "ABC123";

            var accountsResponse = new GetEmployerAccountDetailByHashedIdResponse
            {
                Account = new Models.Account.AccountDetail()
                {
                    AccountId = 123,
                    HashedId = hashedAccountId,
                    Name = "Test 1",
                    LegalEntities = new List<long> { 234, 123 }
                }
            };

            Mediator.Setup(x => x.Send(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountsResponse);
            
             UrlTestHelper.Setup(x => x.RouteUrl(
                 It.Is<UrlRouteContext>(c =>
                     c.RouteName == "GetLegalEntity" && c.Values.IsEquivalentTo(new { hashedAccountId = hashedAccountId, legalEntityId = accountsResponse.Account.LegalEntities[0].ToString() })))
             )
                 .Returns($"/api/accounts/{hashedAccountId}/legalEntity/{accountsResponse.Account.LegalEntities[0]}");
             
             UrlTestHelper.Setup(x => x.RouteUrl(
                     It.Is<UrlRouteContext>(c =>
                         c.RouteName == "GetLegalEntity" && c.Values.IsEquivalentTo(new { hashedAccountId = hashedAccountId, legalEntityId = accountsResponse.Account.LegalEntities[1].ToString() })))
                 )
                 .Returns($"/api/accounts/{hashedAccountId}/legalEntity/{accountsResponse.Account.LegalEntities[1]}");
             
            var response = await Controller.GetAccount(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkObjectResult>(response);
            var model = ((OkObjectResult)response).Value as AccountDetail;

            model.Should().NotBeNull();
            model.AccountId.Should().Be(123);
            model.DasAccountName.Should().Be("Test 1");
            model.HashedAccountId.Should().Be(hashedAccountId);

            foreach (var legalEntity in accountsResponse.Account.LegalEntities)
            {
                var matchedScheme = model.LegalEntities.Single(x => x.Id == legalEntity.ToString());
                matchedScheme?.Href.Should().Be($"/api/accounts/{hashedAccountId}/legalEntity/{legalEntity}");
            }
        }
    }
}
