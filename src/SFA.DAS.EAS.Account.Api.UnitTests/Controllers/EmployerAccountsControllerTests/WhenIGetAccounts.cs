using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.TestCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;

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
                Accounts = new List<Domain.Models.Account.Account>
                    {
                        new Domain.Models.Account.Account {HashedId = "ABC123", Id = 123, Name = "Test 1"},
                        new Domain.Models.Account.Account {HashedId = "ABC999", Id = 987, Name = "Test 2"}
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.Is<GetPagedEmployerAccountsQuery>(q => q.PageNumber == pageNumber && q.PageSize == pageSize && q.ToDate == toDate)))
                    .ReturnsAsync(accountsResponse);

            var balancesResponse = new GetAccountBalancesResponse
            {
                Accounts = new List<AccountBalance>
                    {
                        new AccountBalance {AccountId = accountsResponse.Accounts[0].Id, Balance = 987.65m,IsLevyPayer = 1},
                        new AccountBalance {AccountId = accountsResponse.Accounts[1].Id, Balance = 123.45m,IsLevyPayer = 1}
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.Is<GetAccountBalancesRequest>(q => q.AccountIds.TrueForAll(id => accountsResponse.Accounts.Any(a => a.Id == id)))))
                    .ReturnsAsync(balancesResponse);

            UrlHelper.Setup(x => x.Route("GetAccount", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId = accountsResponse.Accounts[0].HashedId })))).Returns($"/api/accounts/{accountsResponse.Accounts[0].HashedId}");
            UrlHelper.Setup(x => x.Route("GetAccount", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId = accountsResponse.Accounts[1].HashedId })))).Returns($"/api/accounts/{accountsResponse.Accounts[1].HashedId}");

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
                returnedAccount.IsLevyPayer.Should().Be(true);
            }
        }

        [Test]
        public async Task AndNoToDateIsProvidedThenAllAccountsAreReturned()
        {
            await Controller.GetAccounts();

            Mediator.Verify(x => x.SendAsync(It.Is<GetPagedEmployerAccountsQuery>(q => q.ToDate == DateTime.MaxValue.ToString("yyyyMMddHHmmss"))));
        }

        [Test]
        public async Task AndNoPageSizeIsProvidedThen1000AccountsAreReturned()
        {
            await Controller.GetAccounts(DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmss"));

            Mediator.Verify(x => x.SendAsync(It.Is<GetPagedEmployerAccountsQuery>(q => q.PageSize == 1000)));
        }

        [Test]
        public async Task AndNoPageNumberIsProvidedThenTheFirstPageOfAccountsAreReturned()
        {
            await Controller.GetAccounts(DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmss"));

            Mediator.Verify(x => x.SendAsync(It.Is<GetPagedEmployerAccountsQuery>(q => q.PageNumber == 1)));
        }

        [Test]
        public async Task AndAnAccountHasNoBalanceThenTheBalanceIsZero()
        {
            var accountsResponse = new GetPagedEmployerAccountsResponse
            {
                AccountsCount = 1,
                Accounts = new List<Domain.Models.Account.Account>
                    {
                        new Domain.Models.Account.Account {HashedId = "ABC123", Id = 123, Name = "Test 1"}
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>()))
                    .ReturnsAsync(accountsResponse);

            var balancesResponse = new GetAccountBalancesResponse { Accounts = new List<AccountBalance>() };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>()))
                    .ReturnsAsync(balancesResponse);

            var response = await Controller.GetAccounts(DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmss"));
            var model = response as OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>;

            model.Content.Data.First().Balance.Should().Be(0);
        }

        [Test]
        public async Task ThenTheIsLevyPayerFlagDefaultsToTrueIfThereAreNoTransactions()
        {
            //Arrange
            var accountsResponse = new GetPagedEmployerAccountsResponse
            {
                AccountsCount = 2,
                Accounts = new List<Domain.Models.Account.Account>
                    {
                        new Domain.Models.Account.Account {HashedId = "ABC123", Id = 123, Name = "Test 1"},
                        new Domain.Models.Account.Account {HashedId = "ABC999", Id = 987, Name = "Test 2"}
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>()))
                    .ReturnsAsync(accountsResponse);
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>()))
                    .ReturnsAsync(new GetAccountBalancesResponse { Accounts = new List<AccountBalance>() });

            //Act
            var actual = await Controller.GetAccounts();

            //Assert
            var model = actual as OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>;
            Assert.IsNotNull(model);
            Assert.IsNotEmpty(model.Content.Data);
            Assert.IsTrue(model.Content.Data.All(c => c.IsLevyPayer));
        }

        [Test]
        public async Task ThenIfThereIsDataFromTheAccountBalanceQueryThenTheLevyOverFlagIsUsed()
        {
            var accountsResponse = new GetPagedEmployerAccountsResponse
            {
                AccountsCount = 2,
                Accounts = new List<Domain.Models.Account.Account>
                    {
                        new Domain.Models.Account.Account {HashedId = "ABC123", Id = 123, Name = "Test 1"},
                        new Domain.Models.Account.Account {HashedId = "ABC999", Id = 987, Name = "Test 2"}
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>()))
                    .ReturnsAsync(accountsResponse);
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>()))
                    .ReturnsAsync(new GetAccountBalancesResponse { Accounts = new List<AccountBalance> { new AccountBalance { AccountId = 123, Balance = 1, IsLevyPayer = 0 } } });

            //Act
            var actual = await Controller.GetAccounts();

            //Assert
            var model = actual as OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>;
            Assert.IsNotNull(model);
            Assert.IsNotEmpty(model.Content.Data);
            Assert.IsFalse(model.Content.Data.Single(x => x.AccountId.Equals(123)).IsLevyPayer);
            Assert.IsTrue(model.Content.Data.Single(x => x.AccountId.Equals(987)).IsLevyPayer);
        }

        [Test]
        public async Task ThenTheTransferAllowanceShouldBeReturned()
        {
            var expectedTransferAllowance = 50000;
            var accountsResponse = new GetPagedEmployerAccountsResponse
            {
                AccountsCount = 1,
                Accounts = new List<Domain.Models.Account.Account>
                {
                    new Domain.Models.Account.Account
                    {
                        HashedId = "ABC123",
                        Id = 123,
                        Name = "Test 1"
                    }
                }
            };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>()))
                    .ReturnsAsync(accountsResponse);

            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>()))
                    .ReturnsAsync(
                    new GetAccountBalancesResponse
                    {
                        Accounts = new List<AccountBalance>
                        {
                            new AccountBalance
                            {
                                AccountId = 123,
                                Balance=1000,
                                RemainingTransferAllowance = expectedTransferAllowance,
                                StartingTransferAllowance = expectedTransferAllowance + 20000,
                                IsLevyPayer = 1
                            }
                        }
                    });

            //Act
            var actual = await Controller.GetAccounts();

            //Assert
            var model = actual as OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>;
            Assert.IsNotNull(model);
            Assert.IsNotEmpty(model.Content.Data);
            Assert.AreEqual(expectedTransferAllowance, model.Content.Data.First().RemainingTransferAllowance);
        }

        [Test]
        public async Task ThenTheYearlyTransferAllowanceShouldBeReturned()
        {
            var expectedStartingTransferAllowance = 50000;
            var accountsResponse = new GetPagedEmployerAccountsResponse
            {
                AccountsCount = 1,
                Accounts = new List<Domain.Models.Account.Account>
                {
                    new Domain.Models.Account.Account
                    {
                        HashedId = "ABC123",
                        Id = 123,
                        Name = "Test 1"
                    }
                }
            };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>()))
                .ReturnsAsync(accountsResponse);

            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>()))
                .ReturnsAsync(
                    new GetAccountBalancesResponse
                    {
                        Accounts = new List<AccountBalance>
                        {
                            new AccountBalance
                            {
                                AccountId = 123,
                                Balance=1000,
                                RemainingTransferAllowance = expectedStartingTransferAllowance - 20000,
                                StartingTransferAllowance = expectedStartingTransferAllowance,
                                IsLevyPayer = 1
                            }
                        }
                    });

            //Act
            var actual = await Controller.GetAccounts();

            //Assert
            var model = actual as OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>;
            Assert.IsNotNull(model);
            Assert.IsNotEmpty(model.Content.Data);
            Assert.AreEqual(expectedStartingTransferAllowance, model.Content.Data.First().StartingTransferAllowance);
        }
    }
}
