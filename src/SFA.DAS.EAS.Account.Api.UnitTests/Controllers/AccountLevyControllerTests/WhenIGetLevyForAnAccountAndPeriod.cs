﻿using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLevyControllerTests
{
    [TestFixture]
    public class WhenIGetLevyForAnAccountAndPeriod : AccountLevyControllerTests
    {
        [Test]
        public async Task ThenTheLevyIsReturned()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var payrollYear = "2017-18";
            short payrollMonth = 5;         
            var fixture = new Fixture();
            var apiResponse = new List<LevyDeclarationViewModel>() { fixture.Create<LevyDeclarationViewModel>(),  fixture.Create<LevyDeclarationViewModel>() };
            apiResponse[0].HashedAccountId = hashedAccountId;
            apiResponse[1].HashedAccountId = hashedAccountId;
            FinanceApiService.Setup(x => x.GetLevyForPeriod(hashedAccountId, payrollYear, payrollMonth, CancellationToken.None)).ReturnsAsync(apiResponse);

            //Act
            var response = await Controller.GetLevy(hashedAccountId, payrollYear, payrollMonth);
            
            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<ActionResult<AccountResourceList<LevyDeclarationViewModel>>>(response);
            var model = response as ActionResult<AccountResourceList<LevyDeclarationViewModel>>;

            Assert.IsNotNull(model.Result);
            Assert.IsInstanceOf<OkObjectResult>(model.Result);

            var result = (OkObjectResult)model.Result;

            result?.Value.Should().NotBeNull();
            Assert.IsInstanceOf<AccountResourceList<LevyDeclarationViewModel>>(result.Value);

            var value = result.Value as AccountResourceList<LevyDeclarationViewModel>;

            value.Should().NotBeNull();
            Assert.IsTrue(value.TrueForAll(x => x.HashedAccountId == hashedAccountId));
            value.Should().BeEquivalentTo(apiResponse);
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            
            //Act
            var response = await Controller.GetLevy(hashedAccountId, "2017-18", 6);

            var result = response.Result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
