using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.TestCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    [TestFixture]
    public class WhenIGetAnAccount : EmployerAccountsControllerTests
    {
        [Test]
        public async Task ThenTheAccountWithBalanceIsReturned()
        {
            var hashedAccountId = "ABC123";
            var accountResponse = new GetEmployerAccountByHashedIdResponse
            {
                Account = new AccountDetail
                {
                    HashedId = hashedAccountId,
                    AccountId = 123,
                    Name = "Test",
                    OwnerEmail = "test@email.com",
                    CreatedDate = DateTime.Now.AddYears(-1),
                    LegalEntities = new List<long> { 1, 4 },
                    PayeSchemes = new List<string> { "ZZZ/123", "XXX/123" }
                }
            };
            var accountBalanceResponse = new GetAccountBalancesResponse
            {
                Accounts = new List<AccountBalance> { new AccountBalance { AccountId = accountResponse.Account.AccountId, Balance = 123.45m } }
            };
            Mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(accountResponse);
            Mediator.Setup(x => x.SendAsync(It.Is<GetAccountBalancesRequest>(q => q.AccountIds.Single() == accountResponse.Account.AccountId))).ReturnsAsync(accountBalanceResponse);
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferAllowanceQuery>())).ReturnsAsync(new GetTransferAllowanceResponse { TransferAllowance = new TransferAllowance() });

            UrlHelper.Setup(x => x.Route("GetLegalEntity", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, legalEntityId = accountResponse.Account.LegalEntities[0].ToString() }))))
                     .Returns($"/api/accounts/{hashedAccountId}/legalentities/{accountResponse.Account.LegalEntities[0]}");
            UrlHelper.Setup(x => x.Route("GetLegalEntity", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, legalEntityId = accountResponse.Account.LegalEntities[1].ToString() }))))
                     .Returns($"/api/accounts/{hashedAccountId}/legalentities/{accountResponse.Account.LegalEntities[1]}");
            UrlHelper.Setup(x => x.Route("GetPayeScheme", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payeSchemeRef = accountResponse.Account.PayeSchemes[0].Replace(@"/", "%2f") }))))
                     .Returns($"/api/accounts/{hashedAccountId}/payeschemes/{accountResponse.Account.PayeSchemes[0].Replace(@"/", "%2f")}");
            UrlHelper.Setup(x => x.Route("GetPayeScheme", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payeSchemeRef = accountResponse.Account.PayeSchemes[1].Replace(@"/", "%2f") }))))
                     .Returns($"/api/accounts/{hashedAccountId}/payeschemes/{accountResponse.Account.PayeSchemes[1].Replace(@"/", "%2f")}");

            var response = await Controller.GetAccount(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountDetailViewModel>>(response);
            var model = response as OkNegotiatedContentResult<AccountDetailViewModel>;

            model?.Content.Should().NotBeNull();

            model.Content.DasAccountId.Should().Be(hashedAccountId);
            model.Content.HashedAccountId.Should().Be(hashedAccountId);
            model.Content.AccountId.Should().Be(accountResponse.Account.AccountId);
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
            model.Content.Balance.Should().Be(accountBalanceResponse.Accounts.Single().Balance);
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

        [Test]
        public async Task ThenIAmAbleToGetAnAccountByTheInternalId()
        {
            //Arrange
            var accountId = 1923701937;
            var hashedAccountId = "ABC123";
            var accountResponse = new GetEmployerAccountByHashedIdResponse { Account = new AccountDetail() };

            HashingService.Setup(x => x.HashValue(accountId)).Returns(hashedAccountId);
            Mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountByHashedIdQuery>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(accountResponse);
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferAllowanceQuery>())).ReturnsAsync(new GetTransferAllowanceResponse { TransferAllowance = new TransferAllowance() });

            //Act
            var response = await Controller.GetAccount(accountId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountDetailViewModel>>(response);
        }
    }
}
