﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    [TestFixture]
    public class WhenIGetAnAccount : EmployerAccountsControllerTests
    {
        [Test]
        public async Task ThenTheAccountIsReturned()
        {
            var hashedAccountId = "ABC123";
            var accountResponse = new GetEmployerAccountByHashedIdResponse
            {
                Account = new AccountDetail
                    {
                        HashedId = hashedAccountId,
                        Name = "Test",
                        OwnerEmail = "test@email.com",
                        CreatedDate = DateTime.Now.AddYears(-1),
                        LegalEntities = new List<long> { 1, 4 },
                        PayeSchemes = new List<string> { "ZZZ/123", "XXX/123" }
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(accountResponse);

            UrlHelper.Setup(x => x.Route("GetLegalEntity", It.Is<object>(o => o.GetHashCode() == new { hashedAccountId, legalEntityId = accountResponse.Account.LegalEntities[0].ToString() }.GetHashCode()))).Returns($"/api/accounts/{hashedAccountId}/legalentities/{accountResponse.Account.LegalEntities[0]}");
            UrlHelper.Setup(x => x.Route("GetLegalEntity", It.Is<object>(o => o.GetHashCode() == new { hashedAccountId, legalEntityId = accountResponse.Account.LegalEntities[1].ToString() }.GetHashCode()))).Returns($"/api/accounts/{hashedAccountId}/legalentities/{accountResponse.Account.LegalEntities[1]}");
            UrlHelper.Setup(x => x.Route("GetPayeScheme", It.Is<object>(o => o.GetHashCode() == new { hashedAccountId, payeSchemeRef = accountResponse.Account.PayeSchemes[0].Replace(@"/", "%2f") }.GetHashCode()))).Returns($"/api/accounts/{hashedAccountId}/payeschemes/{accountResponse.Account.PayeSchemes[0].Replace(@"/", "%2f")}");
            UrlHelper.Setup(x => x.Route("GetPayeScheme", It.Is<object>(o => o.GetHashCode() == new { hashedAccountId, payeSchemeRef = accountResponse.Account.PayeSchemes[1].Replace(@"/", "%2f") }.GetHashCode()))).Returns($"/api/accounts/{hashedAccountId}/payeschemes/{accountResponse.Account.PayeSchemes[1].Replace(@"/", "%2f")}");

            var response = await Controller.GetAccount(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountDetailViewModel>>(response);
            var model = response as OkNegotiatedContentResult<AccountDetailViewModel>;

            model?.Content.Should().NotBeNull();
            
            model.Content.DasAccountId.Should().Be(hashedAccountId);
            model.Content.DasAccountName.Should().Be(accountResponse.Account.Name);
            model.Content.DateRegistered.Should().Be(accountResponse.Account.CreatedDate);
            model.Content.OwnerEmail.Should().Be(accountResponse.Account.OwnerEmail);
            foreach (var legalEntity in accountResponse.Account.LegalEntities)
            {
                var matchedEntity = model.Content.LegalEntities.Single(x => x.Id == legalEntity.ToString());
                matchedEntity.Href.Should().Be($"/api/accounts/{hashedAccountId}/legalentities/{legalEntity}");
            }
            foreach (var payeScheme in accountResponse.Account.PayeSchemes)
            {
                var matchedScheme = model.Content.PayeSchemes.Single(x => x.Id == payeScheme);
                matchedScheme.Href.Should().Be($"/api/accounts/{hashedAccountId}/payeschemes/{payeScheme.Replace(@"/", "%2f")}");
            }
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";
            var accountResponse = new GetEmployerAccountByHashedIdResponse { Account = null };

            Mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(accountResponse);

            var response = await Controller.GetAccount(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
