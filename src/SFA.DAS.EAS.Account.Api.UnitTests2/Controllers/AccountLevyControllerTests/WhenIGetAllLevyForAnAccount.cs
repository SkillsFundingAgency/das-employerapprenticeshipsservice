using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLevyControllerTests
{
    [TestFixture]
    public class WhenIGetLevyForAnAccount : AccountLevyControllerTests
    {
        [Test]
        public async Task ThenTheLevyIsReturned()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var fixture = new Fixture();
            var apiResponse = new List<LevyDeclarationViewModel>() { fixture.Create<LevyDeclarationViewModel>(), fixture.Create<LevyDeclarationViewModel>() };
            apiResponse[0].HashedAccountId = hashedAccountId;
            apiResponse[1].HashedAccountId = hashedAccountId;
            FinanceApiService.Setup(x => x.GetLevyDeclarations(hashedAccountId)).ReturnsAsync(apiResponse);

            //Act
            var response = await Controller.Index(hashedAccountId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ActionResult<AccountResourceList<LevyDeclarationViewModel>>>(response);
            var model = response as ActionResult<AccountResourceList<LevyDeclarationViewModel>>;

            model?.Value.Should().NotBeNull();
            Assert.IsTrue(model?.Value.TrueForAll(x => x.HashedAccountId == hashedAccountId));
            model?.Value.ShouldAllBeEquivalentTo(apiResponse, options => options.Excluding(x => x.HashedAccountId).Excluding(x => x.PayeSchemeReference));
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            //Arrange
            var hashedAccountId = "ABC123";          

            //Act
            var response = await Controller.Index(hashedAccountId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
