using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Api.Models;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    [TestFixture]
    public class WhenIGetAccounts : EmployerAccountsControllerTests
    {
        [Test]
        public async Task ThenAccountsAreReturnedWithTheirBalanceAndAUriToGetAccountDetails()
        {
            var pageNumber = 123;
            var pageSize = 9084;
            var toDate = DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmss");

            var accountsResponse = new GetPagedEmployerAccountsResponse
            {
                AccountsCount = 2,
                Accounts = new List<Domain.Entities.Account.Account>
                    {
                        new Domain.Entities.Account.Account {HashedId = "ABC123", Id = 123, Name = "Test 1"},
                        new Domain.Entities.Account.Account {HashedId = "ABC999", Id = 987, Name = "Test 2"}
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.Is<GetPagedEmployerAccountsQuery>(q => q.PageNumber == pageNumber && q.PageSize == pageSize && q.ToDate == toDate))).ReturnsAsync(accountsResponse);

            var balancesResponse = new GetAccountBalancesResponse
            {
                Accounts = new List<AccountBalance>
                    {
                        new AccountBalance {AccountId = accountsResponse.Accounts[0].Id, Balance = 987.65m},
                        new AccountBalance {AccountId = accountsResponse.Accounts[1].Id, Balance = 123.45m}
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.Is<GetAccountBalancesRequest>(q => q.AccountIds.TrueForAll(id => accountsResponse.Accounts.Any(a => a.Id == id))))).ReturnsAsync(balancesResponse);

            UrlHelper.Setup(x => x.Link("GetAccount", It.Is<object>(o => o.GetHashCode() == new { hashedAccountId = accountsResponse.Accounts[0].HashedId }.GetHashCode()))).Returns($"/api/accounts/{accountsResponse.Accounts[0].HashedId}");
            UrlHelper.Setup(x => x.Link("GetAccount", It.Is<object>(o => o.GetHashCode() == new { hashedAccountId = accountsResponse.Accounts[1].HashedId }.GetHashCode()))).Returns($"/api/accounts/{accountsResponse.Accounts[1].HashedId}");

            var response = await Controller.GetAccounts(toDate, pageSize, pageNumber);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>>(response);
            var model = response as OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>;

            model?.Content?.Data.Should().NotBeNull();
            model.Content.Page.Should().Be(pageNumber);
            model.Content.Data.Should().HaveCount(accountsResponse.AccountsCount);
            foreach (var expectedAccount in accountsResponse.Accounts)
            {
                var returnedAccount = model.Content.Data.SingleOrDefault(x => x.AccountId == expectedAccount.Id && x.AccountHashId == expectedAccount.HashedId && x.AccountName == expectedAccount.Name);
                returnedAccount.Should().NotBeNull();
                returnedAccount.Balance.Should().Be(balancesResponse.Accounts.Single(b => b.AccountId == returnedAccount.AccountId).Balance);
                returnedAccount.Href.Should().Be($"/api/accounts/{returnedAccount.AccountHashId}");
            }
        }

        [Test]
        public async Task AndNoToDateIsProvidedThenAllAccountsAreReturned()
        {
            var accountsResponse = new GetPagedEmployerAccountsResponse { Accounts = new List<Domain.Entities.Account.Account>()};
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>())).ReturnsAsync(accountsResponse);

            var balancesResponse = new GetAccountBalancesResponse { Accounts = new List<AccountBalance>() };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>())).ReturnsAsync(balancesResponse);

            await Controller.GetAccounts();

            Mediator.Verify(x => x.SendAsync(It.Is<GetPagedEmployerAccountsQuery>(q => q.ToDate == DateTime.MaxValue.ToString("yyyyMMddHHmmss"))));
        }

        [Test]
        public async Task AndNoPageSizeIsProvidedThen1000AccountsAreReturned()
        {
            var accountsResponse = new GetPagedEmployerAccountsResponse { Accounts = new List<Domain.Entities.Account.Account>() };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>())).ReturnsAsync(accountsResponse);

            var balancesResponse = new GetAccountBalancesResponse { Accounts = new List<AccountBalance>() };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>())).ReturnsAsync(balancesResponse);

            await Controller.GetAccounts(DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmss"));

            Mediator.Verify(x => x.SendAsync(It.Is<GetPagedEmployerAccountsQuery>(q => q.PageSize == 1000)));
        }

        [Test]
        public async Task AndNoPageNumberIsProvidedThenTheFirstPageOfAccountsAreReturned()
        {
            var accountsResponse = new GetPagedEmployerAccountsResponse { Accounts = new List<Domain.Entities.Account.Account>() };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>())).ReturnsAsync(accountsResponse);

            var balancesResponse = new GetAccountBalancesResponse { Accounts = new List<AccountBalance>() };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>())).ReturnsAsync(balancesResponse);

            await Controller.GetAccounts(DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmss"));

            Mediator.Verify(x => x.SendAsync(It.Is<GetPagedEmployerAccountsQuery>(q => q.PageNumber == 1)));
        }

        [Test]
        public async Task AndAnAccountHasNoBalanceThenTheBalanceIsZero()
        {
            var accountsResponse = new GetPagedEmployerAccountsResponse
            {
                AccountsCount = 1,
                Accounts = new List<Domain.Entities.Account.Account>
                    {
                        new Domain.Entities.Account.Account {HashedId = "ABC123", Id = 123, Name = "Test 1"}
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>())).ReturnsAsync(accountsResponse);

            var balancesResponse = new GetAccountBalancesResponse { Accounts = new List<AccountBalance>() };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>())).ReturnsAsync(balancesResponse);

            var response = await Controller.GetAccounts(DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmss"));
            var model = response as OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>;

            model.Content.Data.First().Balance.Should().Be(0);
        }
    }
}
