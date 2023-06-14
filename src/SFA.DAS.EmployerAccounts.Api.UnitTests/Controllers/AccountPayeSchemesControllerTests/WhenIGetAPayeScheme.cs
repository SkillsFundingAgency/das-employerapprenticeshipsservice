using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.TestCommon.Extensions;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.AccountPayeSchemesControllerTests;

[TestFixture]
public class WhenIGetAPayeScheme
{
    [Test, MoqAutoData]
    public async Task ThenThePayeSchemesAreReturned(
        string hashedAccountId,
        long accountId,
        GetAccountPayeSchemesResponse accountResponse,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IEncodingService> encodingServiceMock,
        [Frozen] Mock<IUrlHelper> urlHelperMock,
        [NoAutoProperties] AccountPayeSchemesController sut)
    {
        accountResponse.PayeSchemes.RemoveAt(2);

        sut.Url = urlHelperMock.Object;

        mediatorMock
            .Setup(x => x.Send(It.Is<GetAccountPayeSchemesQuery>(q => q.AccountId == accountId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountResponse);

        encodingServiceMock
            .Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId))
            .Returns(accountId);

        foreach (var scheme in accountResponse.PayeSchemes)
        {
            scheme.Ref = $"{RandomNumberGenerator.GetInt32(100, 999)}/REF";

            urlHelperMock
                .Setup(
                    x => x.RouteUrl(
                        It.Is<UrlRouteContext>(c =>
                            c.RouteName == "GetPayeScheme" &&
                            c.Values.IsEquivalentTo(new
                            {
                                hashedAccountId,
                                payeSchemeRef = Uri.EscapeDataString(scheme.Ref)
                            })))
                ).Returns($"/api/accounts/{hashedAccountId}/payeschemes/scheme?payeSchemeRef={scheme.Ref.Replace(@"/", "%2f")}");
        }

        var response = await sut.GetPayeSchemes(hashedAccountId);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<OkObjectResult>(response);

        var model = ((OkObjectResult)response).Value as ResourceList;

        model.Should().NotBeNull();

        foreach (var payeScheme in accountResponse.PayeSchemes)
        {
            var matchedScheme = model.Single(x => x.Id == payeScheme.Ref);
            matchedScheme?.Href.Should()
                .Be($"/api/accounts/{hashedAccountId}/payeschemes/scheme?payeSchemeRef={payeScheme.Ref.Replace(@"/", "%2f")}");
        }
    }

    [Test, MoqAutoData]
    public async Task AndTheAccountDoesNotExistThenItIsNotReturned(
        string hashedAccountId,
        [NoAutoProperties] AccountPayeSchemesController sut)
    {
        var response = await sut.GetPayeSchemes(hashedAccountId);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<NotFoundResult>(response);
    }

    [Test, MoqAutoData]
    public async Task ThenTheAccountIsReturned(
        string hashedAccountId,
        string payeSchemeRef,
        GetPayeSchemeByRefResponse payeSchemeResponse,
        [Frozen] Mock<IMediator> mediatorMock,
        [NoAutoProperties] AccountPayeSchemesController sut)
    {
        mediatorMock.Setup(x =>
            x.Send(
                It.Is<GetPayeSchemeByRefQuery>(q => q.Ref == payeSchemeRef && q.HashedAccountId == hashedAccountId),
                It.IsAny<CancellationToken>())).ReturnsAsync(payeSchemeResponse);

        var response = await sut.GetPayeScheme(hashedAccountId, payeSchemeRef.Replace("/", "%2f")) as OkObjectResult;

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<OkObjectResult>(response);

        var model = response.Value as PayeScheme;

        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(payeSchemeResponse.PayeScheme);
        model?.DasAccountId.Should().Be(hashedAccountId);
    }

    [Test, MoqAutoData]
    public async Task AndThePayeSchemeDoesNotExistThenItIsNotReturned(
        string hashedAccountId,
        string payeSchemeRef,
        GetPayeSchemeByRefResponse payeSchemeResponse,
        [Frozen] Mock<IMediator> mediatorMock,
        [NoAutoProperties] AccountPayeSchemesController sut)
    {
        payeSchemeResponse.PayeScheme = null;

        mediatorMock
            .Setup(x => x.Send(
                It.Is<GetPayeSchemeByRefQuery>(q => q.Ref == payeSchemeRef && q.HashedAccountId == hashedAccountId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(payeSchemeResponse);

        var response = await sut.GetPayeScheme(hashedAccountId, payeSchemeRef);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<NotFoundResult>(response);
    }
}