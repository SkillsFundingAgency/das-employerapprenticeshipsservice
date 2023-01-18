using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
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

            Microsoft.AspNetCore.Mvc.Routing.UrlHelper.Setup(x => x.Route("GetPayeScheme", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payeSchemeRef = accountsResponse.Account.PayeSchemes[0].Replace("/", "%2f") })))).Returns($"/api/accounts/{hashedAccountId}/payescheme/{accountsResponse.Account.PayeSchemes[0].Replace("/", "%2f")}");
            Microsoft.AspNetCore.Mvc.Routing.UrlHelper.Setup(x => x.Route("GetPayeScheme", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payeSchemeRef = accountsResponse.Account.PayeSchemes[1].Replace("/", "%2f") })))).Returns($"/api/accounts/{hashedAccountId}/payescheme/{accountsResponse.Account.PayeSchemes[1].Replace("/", "%2f")}");

            var response = await Controller.GetAccount(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountDetail>>(response);
            var model = response as OkNegotiatedContentResult<AccountDetail>;

            model?.Content?.Should().NotBeNull();
            model?.Content?.AccountId.Should().Be(123);
            model?.Content?.DasAccountName.Should().Be("Test 1");
            model?.Content?.HashedAccountId.Should().Be(hashedAccountId);

            foreach (var payeScheme in accountsResponse.Account.PayeSchemes)
            {
                var matchedScheme = model?.Content?.PayeSchemes.Single(x => x.Id == payeScheme);
                matchedScheme?.Href.Should().Be($"/api/accounts/{hashedAccountId}/payescheme/{payeScheme.Replace("/", "%2f")}");
            }
        }
    }
}
