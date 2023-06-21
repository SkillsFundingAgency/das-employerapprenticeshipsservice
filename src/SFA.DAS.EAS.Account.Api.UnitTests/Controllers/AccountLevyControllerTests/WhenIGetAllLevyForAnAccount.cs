using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLevyControllerTests;

[TestFixture]
public class WhenIGetLevyForAnAccount : AccountLevyControllerTests
{
    [Test]
    public async Task ThenTheLevyIsReturned()
    {
        //Arrange
        const string hashedAccountId = "ABC123";
        var fixture = new Fixture();
        var apiResponse = new List<LevyDeclarationViewModel>() { fixture.Create<LevyDeclarationViewModel>(), fixture.Create<LevyDeclarationViewModel>() };
        apiResponse[0].HashedAccountId = hashedAccountId;
        apiResponse[1].HashedAccountId = hashedAccountId;
        FinanceApiService!.Setup(x => x.GetLevyDeclarations(hashedAccountId, CancellationToken.None)).ReturnsAsync(apiResponse);

        //Act
        var response = await Controller!.Index(hashedAccountId);

        //Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.InstanceOf<ActionResult<AccountResourceList<LevyDeclarationViewModel>>>());
        var model = response as ActionResult<AccountResourceList<LevyDeclarationViewModel>>;

        Assert.That(model.Result, Is.Not.Null);
        Assert.That(model.Result, Is.InstanceOf<OkObjectResult>());

        var result = (OkObjectResult)model.Result!;

        result?.Value.Should().NotBeNull();
        Assert.That(result!.Value, Is.InstanceOf<AccountResourceList<LevyDeclarationViewModel>>());
            
        var value = result.Value as AccountResourceList<LevyDeclarationViewModel>;

        Assert.That(value!.TrueForAll(x => x.HashedAccountId == hashedAccountId), Is.True);
        value.Should().BeEquivalentTo(apiResponse);
    }

    [Test]
    public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
    {
        //Arrange
        const string hashedAccountId = "ABC123";          

        //Act
        var response = await Controller!.Index(hashedAccountId);

        var result = response.Result;

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}