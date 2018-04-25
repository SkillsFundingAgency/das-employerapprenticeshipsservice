﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.TestCommon.Extensions;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetLegalEntitiesForAnAccount : AccountLegalEntitiesControllerTests
    {
        [Test]
        public async Task ThenTheLegalEntitiesAreReturned()
        {
            var hashedAccountId = "ABC123";
            var accountResponse = new GetEmployerAccountByHashedIdResponse { Account = new AccountDetail { LegalEntities = new List<long> { 1, 4 }, PayeSchemes = new List<string> () } };
            Mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(accountResponse);

            UrlHelper.Setup(x => x.Route("GetLegalEntity", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, legalEntityId = accountResponse.Account.LegalEntities[0].ToString() })))).Returns($"/api/accounts/{hashedAccountId}/legalentities/{accountResponse.Account.LegalEntities[0]}");
            UrlHelper.Setup(x => x.Route("GetLegalEntity", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, legalEntityId = accountResponse.Account.LegalEntities[1].ToString() })))).Returns($"/api/accounts/{hashedAccountId}/legalentities/{accountResponse.Account.LegalEntities[1]}");
            
            var response = await Controller.GetLegalEntities(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ResourceList>>(response);
            var model = response as OkNegotiatedContentResult<ResourceList>;

            model?.Content.Should().NotBeNull();
            
            foreach (var legalEntity in accountResponse.Account.LegalEntities)
            {
                var matchedEntity = model.Content.Single(x => x.Id == legalEntity.ToString());
                matchedEntity.Href.Should().Be($"/api/accounts/{hashedAccountId}/legalentities/{legalEntity}");
            }
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";
            var accountResponse = new GetEmployerAccountByHashedIdResponse { Account = null };

            Mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(accountResponse);

            var response = await Controller.GetLegalEntities(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
