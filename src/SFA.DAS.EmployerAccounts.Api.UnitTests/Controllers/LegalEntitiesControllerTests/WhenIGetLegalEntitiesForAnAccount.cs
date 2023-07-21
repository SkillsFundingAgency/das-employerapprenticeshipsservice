using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Mappings;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;
using SFA.DAS.EmployerAccounts.TestCommon.Extensions;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.LegalEntitiesControllerTests;

[TestFixture]
public class WhenIGetLegalEntitiesForAnAccount : LegalEntitiesControllerTests
{
    private string _hashedAccountId;
    private GetAccountLegalEntitiesByHashedAccountIdResponse _response;

    [Test]
    public async Task ThenTheLegalEntitiesAreReturned()
    {
        _hashedAccountId = "ABC123";
        _response = new GetAccountLegalEntitiesByHashedAccountIdResponse
        {
            LegalEntities =
                new List<AccountLegalEntity>
                {
                    new AccountLegalEntity
                    {
                        Id = 1,
                        LegalEntityId = 5
                    },
                    new AccountLegalEntity
                    {
                        Id = 4,
                        LegalEntityId = 9
                    }
                }
        };

        Mediator.Setup(x =>
            x.Send(It.Is<GetAccountLegalEntitiesByHashedAccountIdRequest>(q => q.HashedAccountId == _hashedAccountId),
                It.IsAny<CancellationToken>())).ReturnsAsync(_response);

        SetupUrlHelperForAccountLegalEntityOne();
        SetupUrlHelperForAccountLegalEntityTwo();

        var response = await Controller.GetLegalEntities(_hashedAccountId);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<OkObjectResult>(response);
        var model = ((OkObjectResult)response).Value as Types.ResourceList;

        model.Should().NotBeNull();

        foreach (var legalEntity in _response.LegalEntities)
        {
            var matchedEntity = model.Single(x => x.Id == legalEntity.LegalEntityId.ToString());
            matchedEntity.Href.Should()
                .Be($"/api/accounts/{_hashedAccountId}/legalentities/{legalEntity.LegalEntityId}");
        }
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_If_Set_To_Include_Details_Then_AccountLegalEntity_List_Returned(
        List<AccountLegalEntity> legalEntities)
    {
        var expectedModel = legalEntities.Select(c => LegalEntityMapping.MapFromAccountLegalEntity(c, null, false))
            .ToList();
        _hashedAccountId = "ABC123";
        _response = new GetAccountLegalEntitiesByHashedAccountIdResponse
        {
            LegalEntities = legalEntities
        };

        Mediator.Setup(x =>
            x.Send(It.Is<GetAccountLegalEntitiesByHashedAccountIdRequest>(q => q.HashedAccountId == _hashedAccountId),
                It.IsAny<CancellationToken>())).ReturnsAsync(_response);

        SetupUrlHelperForAccountLegalEntityOne();
        SetupUrlHelperForAccountLegalEntityTwo();

        var response = await Controller.GetLegalEntities(_hashedAccountId, true);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<OkObjectResult>(response);
        var model = ((OkObjectResult)response).Value as List<Types.LegalEntity>;
        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(expectedModel);
    }

    [Test]
    public async Task AndTheAccountCannotBeDecodedThenItIsNotReturned()
    {
        Mediator.Setup(
                x => x.Send(
                    It.Is<GetAccountLegalEntitiesByHashedAccountIdRequest>(q => q.HashedAccountId == _hashedAccountId),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

        var response = await Controller.GetLegalEntities(_hashedAccountId);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<NotFoundResult>(response);
    }

    [Test]
    public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
    {
        Mediator.Setup(
                x => x.Send(
                    It.Is<GetAccountLegalEntitiesByHashedAccountIdRequest>(q => q.HashedAccountId == _hashedAccountId),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new GetAccountLegalEntitiesByHashedAccountIdResponse
                {
                    LegalEntities = new List<AccountLegalEntity>(0)
                });

        var response = await Controller.GetLegalEntities(_hashedAccountId);

        Assert.IsNotNull(response);
        Assert.IsInstanceOf<NotFoundResult>(response);
    }

    private void SetupUrlHelperForAccountLegalEntityOne()
    {
        UrlTestHelper.Setup(
                x => x.RouteUrl(
                    It.Is<UrlRouteContext>(c => c.RouteName == "GetLegalEntity" && c.Values.IsEquivalentTo(new
                    {
                        hashedAccountId =_hashedAccountId,
                        legalEntityId = _response.LegalEntities[0].LegalEntityId
                    }))))
            .Returns($"/api/accounts/{_hashedAccountId}/legalentities/{_response.LegalEntities[0].LegalEntityId}");
    }

    private void SetupUrlHelperForAccountLegalEntityTwo()
    {
        UrlTestHelper.Setup(
                x => x.RouteUrl(
                    It.Is<UrlRouteContext>(c => c.RouteName == "GetLegalEntity" && c.Values.IsEquivalentTo(new
                    {
                        hashedAccountId =_hashedAccountId,
                        legalEntityId = _response.LegalEntities[1].LegalEntityId
                    }))))
            .Returns($"/api/accounts/{_hashedAccountId}/legalentities/{_response.LegalEntities[1].LegalEntityId}");
    }
}