using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.TestCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AccountWithBalanceViewModel = SFA.DAS.EAS.Account.Api.Types.AccountWithBalanceViewModel;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    [TestFixture]
    public class WhenIGetAccounts : EmployerAccountsControllerTests
    {
        [Test]
        public async Task ThenAccountsAreReturnedWithTheirBalanceAndAUriToGetAccountDetails()
        {
            var accountsResponse = new PagedApiResponseViewModel<AccountWithBalanceViewModel>()
            {
                Page = 123,
                TotalPages = 123,
                Data = new List<AccountWithBalanceViewModel>
                {
                    new AccountWithBalanceViewModel { AccountHashId = "ABC123", AccountId = 123, AccountName = "Test 1", IsLevyPayer = true },
                    new AccountWithBalanceViewModel { AccountHashId = "ABC999", AccountId = 987, AccountName = "Test 2", IsLevyPayer = true }
                }
            };

            ApiService.Setup(s => s.GetAccounts(null, 1000, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountsResponse);

            var result = await Controller.GetAccounts();

            result.Should().NotBeNull();
            result.Should().BeOfType<OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>>();

            var okResult = (OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>) result;
            okResult.Content.Should().NotBeNull();
            okResult.Content.ShouldBeEquivalentTo(accountsResponse);
        }
    }
}
