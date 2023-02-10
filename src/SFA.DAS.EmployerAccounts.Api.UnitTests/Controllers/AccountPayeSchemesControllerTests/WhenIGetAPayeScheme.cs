using System;
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
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.TestCommon.Extensions;
using PayeScheme = SFA.DAS.EmployerAccounts.Api.Types.PayeScheme;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.AccountPayeSchemesControllerTests;

[TestFixture]
public class WhenIGetAPayeScheme : AccountPayeSchemesControllerTests
{
    private long _accountId;
    private string _hashedAccountId;
    private GetAccountPayeSchemesResponse _accountResponse;

    [Test]
    public async Task ThenThePayeSchemesAreReturned()
    {
        _accountId = 123;
        _hashedAccountId = "ABC123";
        _accountResponse = new GetAccountPayeSchemesResponse
        {
            PayeSchemes =
                new List<PayeView>
                {
                    new()
                    {
                        Ref = "ABC/123"
                    },
                    new()
                    {
                        Ref = "ZZZ/999"
                    }
                }
        };

        Mediator.Setup(x => x.Send(It.Is<GetAccountPayeSchemesQuery>(q => q.AccountId == _accountId),
            It.IsAny<CancellationToken>())).ReturnsAsync(_accountResponse);

        UrlTestHelper
            .Setup(
                x => x.RouteUrl(
                    It.Is<UrlRouteContext>(c =>
                        c.RouteName == "GetPayeScheme" &&
                        c.Values.IsEquivalentTo(new
                        {
                            hashedAccountId = _accountId,
                            payeSchemeRef = WebUtility.UrlEncode(_accountResponse.PayeSchemes[0].Ref)
                        })))
            )
            .Returns(
                $"/api/accounts/{_accountId}/payeschemes/{_accountResponse.PayeSchemes[0].Ref.Replace(@"/", "%2f")}");

        UrlTestHelper
            .Setup(
                x => x.RouteUrl(
                    It.Is<UrlRouteContext>(c =>
                        c.RouteName == "GetPayeScheme" &&
                        c.Values.IsEquivalentTo(new
                        {
                            hashedAccountId = _accountId,
                            payeSchemeRef = WebUtility.UrlEncode(_accountResponse.PayeSchemes[1].Ref)
                        })))
            )
            .Returns(
                $"/api/accounts/{_accountId}/payeschemes/{_accountResponse.PayeSchemes[1].Ref.Replace(@"/", "%2f")}");

        var response = await Controller.GetPayeSchemes(_hashedAccountId);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<OkObjectResult>(response);

        var model = ((OkObjectResult)response).Value as ResourceList;

        model.Should().NotBeNull();

        foreach (var payeScheme in _accountResponse.PayeSchemes)
        {
            var matchedScheme = model.Single(x => x.Id == payeScheme.Ref);
            matchedScheme?.Href.Should()
                .Be($"/api/accounts/{_accountId}/payeschemes/{payeScheme.Ref.Replace(@"/", "%2f")}");
        }
    }

    [Test]
    public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
    {
        var hashedAccountId = "ABC123";
        var accountId = 123;
        var accountResponse = new GetAccountPayeSchemesResponse();

        Mediator.Setup(x => x.Send(It.Is<GetAccountPayeSchemesQuery>(q => q.AccountId == accountId),
            It.IsAny<CancellationToken>())).ReturnsAsync(accountResponse);

        var response = await Controller.GetPayeSchemes(hashedAccountId);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<NotFoundResult>(response);
    }

    [Test]
    public async Task AndTheAccountCannotBeDecodedThenItIsNotReturned()
    {
        var hashedAccountId = "ABC123";
        var accountId = 123;

        Mediator.Setup(
                x => x.Send(It.Is<GetAccountPayeSchemesQuery>(q => q.AccountId == accountId),
                    It.IsAny<CancellationToken>()))
            .Throws(new InvalidRequestException(new Dictionary<string, string>()));

        var response = await Controller.GetPayeSchemes(hashedAccountId);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<NotFoundResult>(response);
    }

    [Test]
    public async Task ThenTheAccountIsReturned()
    {
        var hashedAccountId = "ABC123";
        var payeSchemeRef = "ZZZ/123";
        var payeSchemeResponse = new GetPayeSchemeByRefResponse
        {
            PayeScheme = new PayeSchemeView
            {
                Ref = payeSchemeRef,
                Name = "Test",
                AddedDate = DateTime.Now.AddYears(-10),
                RemovedDate = DateTime.Now
            }
        };
        Mediator.Setup(x =>
            x.Send(
                It.Is<GetPayeSchemeByRefQuery>(q => q.Ref == payeSchemeRef && q.HashedAccountId == hashedAccountId),
                It.IsAny<CancellationToken>())).ReturnsAsync(payeSchemeResponse);

        var response =
            await Controller.GetPayeScheme(hashedAccountId, payeSchemeRef.Replace("/", "%2f")) as OkObjectResult;

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<OkObjectResult>(response);

        var model = response.Value as PayeScheme;

        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(payeSchemeResponse.PayeScheme);
        model?.DasAccountId.Should().Be(hashedAccountId);
    }

    [Test]
    public async Task AndThePayeSchemeDoesNotExistThenItIsNotReturned()
    {
        var hashedAccountId = "ABC123";
        var payeSchemeRef = "ZZZ/123";
        var payeSchemeResponse = new GetPayeSchemeByRefResponse { PayeScheme = null };

        Mediator.Setup(x =>
            x.Send(
                It.Is<GetPayeSchemeByRefQuery>(q => q.Ref == payeSchemeRef && q.HashedAccountId == hashedAccountId),
                It.IsAny<CancellationToken>())).ReturnsAsync(payeSchemeResponse);

        var response = await Controller.GetPayeScheme(hashedAccountId, payeSchemeRef);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<NotFoundResult>(response);
    }
}