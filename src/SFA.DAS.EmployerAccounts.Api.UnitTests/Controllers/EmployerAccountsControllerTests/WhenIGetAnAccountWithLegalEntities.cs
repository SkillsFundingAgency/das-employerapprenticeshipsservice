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

            Microsoft.AspNetCore.Mvc.Routing.UrlHelper.Setup(x => x.Route("GetLegalEntity", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, legalEntityId = accountsResponse.Account.LegalEntities[0] })))).Returns($"/api/accounts/{hashedAccountId}/legalEntity/{accountsResponse.Account.LegalEntities[0]}");
            Microsoft.AspNetCore.Mvc.Routing.UrlHelper.Setup(x => x.Route("GetLegalEntity", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, legalEntityId = accountsResponse.Account.LegalEntities[1] })))).Returns($"/api/accounts/{hashedAccountId}/legalEntity/{accountsResponse.Account.LegalEntities[1]}");

            var response = await Controller.GetAccount(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountDetail>>(response);
            var model = response as OkNegotiatedContentResult<AccountDetail>;

            model?.Content?.Should().NotBeNull();
            model?.Content?.AccountId.Should().Be(123);
            model?.Content?.DasAccountName.Should().Be("Test 1");
            model?.Content?.HashedAccountId.Should().Be(hashedAccountId);

            foreach (var legalEntity in accountsResponse.Account.LegalEntities)
            {
                var matchedScheme = model?.Content?.LegalEntities.Single(x => x.Id == legalEntity.ToString());
                matchedScheme?.Href.Should().Be($"/api/accounts/{hashedAccountId}/legalEntity/{legalEntity}");
            }
        }
    }
}
