using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions.Common;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAUsersAccounts : ApiClientTestBase
    {
        private AccountDetailViewModel _accountViewModel;
        private string _uri;

        public override void HttpClientSetup()
        {
            _uri = $"/api/user/{TextualAccountId}/accounts";
            var absoluteUri = Configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            _accountViewModel = new AccountDetailViewModel
            {
                HashedAccountId = "123ABC",
                AccountId = 123,
                DasAccountName = "Test Account",
                DateRegistered = DateTime.Now.AddDays(-30),
            };

            var accounts = new List<AccountDetailViewModel> { _accountViewModel };

            HttpClient.Setup(c => c.GetAsync(absoluteUri))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(accounts)));
        }

        [Test]
        public async Task ThenThePayeSchemeIsReturned()
        {
            // Act
            var response = await ApiClient.GetUserAccounts(TextualAccountId);
            var account = response?.FirstOrDefault();

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(account);
            account.IsSameOrEqualTo(_accountViewModel);
        }
    }
}
