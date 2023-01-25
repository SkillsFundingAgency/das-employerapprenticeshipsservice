using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.EmployerAccounts.TestCommon.Extensions;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
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

            Mediator.Setup(x => x.Send(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountsResponse);

            UrlTestHelper.Setup(x => x.RouteUrl("GetPayeScheme", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payeSchemeRef = accountsResponse.Account.PayeSchemes[0].Replace("/", "%2f") })))).Returns($"/api/accounts/{hashedAccountId}/payescheme/{accountsResponse.Account.PayeSchemes[0].Replace("/", "%2f")}");
            UrlTestHelper.Setup(x => x.RouteUrl("GetPayeScheme", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payeSchemeRef = accountsResponse.Account.PayeSchemes[1].Replace("/", "%2f") })))).Returns($"/api/accounts/{hashedAccountId}/payescheme/{accountsResponse.Account.PayeSchemes[1].Replace("/", "%2f")}");

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
                matchedScheme?.Href.Should().Be($"/api/accounts/{hashedAccountId}/payescheme/{payeScheme.Replace("/", "%2f")}");
            }
        }
    }
}
