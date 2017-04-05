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
    public class WhenGettingAUsersAccounts
    {
        private AccountApiConfiguration _configuration;
        private Mock<SecureHttpClient> _httpClient;
        private AccountApiClient _apiClient;
        private AccountDetailViewModel _accountViewModel;
        private string _uri;
        private string _userId;

        [SetUp]
        public void Arrange()
        {
            _userId = "ABC123";

            _configuration = new AccountApiConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };

            _uri = $"/api/user/{_userId}/accounts";
            var absoluteUri = _configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            _accountViewModel = new AccountDetailViewModel
            {
               HashedAccountId = "123ABC",
               AccountId = 123,
               DasAccountName = "Test Account",
               DateRegistered = DateTime.Now.AddDays(-30),
            };

            var accounts = new List<AccountDetailViewModel> { _accountViewModel };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(absoluteUri))
                       .Returns(Task.FromResult(JsonConvert.SerializeObject(accounts)));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenThePayeSchemeIsReturned()
        {
            // Act
            var response = await _apiClient.GetUserAccounts(_userId);
            var account = response?.FirstOrDefault();

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(account);
            account.IsSameOrEqualTo(_accountViewModel);
        }
    }
}
