using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.EmployerAccountsControllerTests;

public class WhenIGetAnAccountWithPayeSchemes : EmployerAccountsControllerTests
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
                PayeSchemes = new List<string> { "123/4567", "345/6554" }
            }
        };

        Mediator.Setup(x =>
                x.Send(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountsResponse);

        UrlTestHelper
            .Setup(
                x => x.RouteUrl(
                    It.Is<UrlRouteContext>(
                        c => c.RouteName == "GetPayeScheme" && c.Values.IsEquivalentTo(new
                        {
                            hashedAccountId,
                            payeSchemeRef = WebUtility.UrlEncode(accountsResponse.Account.PayeSchemes[0])
                        })))
            )
            .Returns(
                $"/api/accounts/{hashedAccountId}/payeschemes/scheme?ref={accountsResponse.Account.PayeSchemes[0].Replace("/", "%2f")}");

        UrlTestHelper
            .Setup(
                x => x.RouteUrl(
                    It.Is<UrlRouteContext>(
                        c => c.RouteName == "GetPayeScheme" && c.Values.IsEquivalentTo(new
                        {
                            hashedAccountId,
                            payeSchemeRef = WebUtility.UrlEncode(accountsResponse.Account.PayeSchemes[1])
                        })))
            )
            .Returns(
                $"/api/accounts/{hashedAccountId}/payeschemes/scheme?ref={accountsResponse.Account.PayeSchemes[1].Replace("/", "%2f")}");

        var response = await Controller.GetAccount(hashedAccountId);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<OkObjectResult>(response);
        var model = ((OkObjectResult)response).Value as AccountDetail;

        model.Should().NotBeNull();
        model.AccountId.Should().Be(123);
        model.DasAccountName.Should().Be("Test 1");
        model.HashedAccountId.Should().Be(hashedAccountId);

        foreach (var payeScheme in accountsResponse.Account.PayeSchemes)
        {
            var matchedScheme = model.PayeSchemes.Single(x => x.Id == payeScheme);
            matchedScheme?.Href.Should()
                .Be($"/api/accounts/{hashedAccountId}/payeschemes/scheme?ref={payeScheme.Replace("/", "%2f")}");
        }
    }
}