using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.TestCommon.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountPayeSchemesControllerTests
{
    [TestFixture]
    public class WhenIGetPayeSchemesForAnAccount : AccountPayeSchemesControllerTests
    {
        [Test]
        public async Task ThenThePayeSchemesAreReturned()
        {
            var hashedAccountId = "ABC123";
            var accountResponse = new GetEmployerAccountByHashedIdResponse { Account = new AccountDetail { LegalEntities = new List<long>(), PayeSchemes = new List<string> { "ABC/123", "ZZZ/999" } } };
            Mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(accountResponse);

            Mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferAllowanceQuery>())).ReturnsAsync(new GetTransferAllowanceResponse { TransferAllowance = new TransferAllowance() });

            UrlHelper.Setup(x => x.Route("GetPayeScheme", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payeSchemeRef = accountResponse.Account.PayeSchemes[0].Replace(@"/", "%2f") })))).Returns($"/api/accounts/{hashedAccountId}/payeschemes/{accountResponse.Account.PayeSchemes[0].Replace(@"/", "%2f")}");
            UrlHelper.Setup(x => x.Route("GetPayeScheme", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payeSchemeRef = accountResponse.Account.PayeSchemes[1].Replace(@"/", "%2f") })))).Returns($"/api/accounts/{hashedAccountId}/payeschemes/{accountResponse.Account.PayeSchemes[1].Replace(@"/", "%2f")}");

            var response = await Controller.GetPayeSchemes(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ResourceList>>(response);
            var model = response as OkNegotiatedContentResult<ResourceList>;

            model?.Content.Should().NotBeNull();

            foreach (var payeScheme in accountResponse.Account.PayeSchemes)
            {
                var matchedScheme = model.Content.Single(x => x.Id == payeScheme);
                matchedScheme.Href.Should().Be($"/api/accounts/{hashedAccountId}/payeschemes/{payeScheme.Replace(@"/", "%2f")}");
            }
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";
            var accountResponse = new GetEmployerAccountByHashedIdResponse { Account = null };

            Mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(accountResponse);

            var response = await Controller.GetPayeSchemes(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
