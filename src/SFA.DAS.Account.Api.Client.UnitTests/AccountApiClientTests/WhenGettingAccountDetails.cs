using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAccountDetails
    {
        private AccountApiConfiguration _configuration;
        private Mock<SecureHttpClient> _httpClient;
        private AccountApiClient _apiClient;
        private AccountDetailViewModel _expectedAccount;
        private string _uri;

        [SetUp]
        public void Arrange()
        {
            _configuration = new AccountApiConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };

            _uri = "http://localhost/api/accounts/ABC123";
            _expectedAccount = new AccountDetailViewModel
            {
                DasAccountId = "1",
                DasAccountName = "Account 1",
                DateRegistered = DateTime.Now.AddYears(-1),
                OwnerEmail = "test@email.com",
                LegalEntities = new List<ResourceViewModel>() { new ResourceViewModel { Id = "1", Href = "/api/legalentities/test1" } },
                PayeSchemes = new List<ResourceViewModel>() { new ResourceViewModel { Id = "1", Href = "/api/payeschemes/test1" } }
            };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(_uri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_expectedAccount)));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenTheAccountDetailsAreReturned()
        {
            // Act
            var response = await _apiClient.GetResource<AccountDetailViewModel>(_uri);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<AccountDetailViewModel>(response);
            response.Should().NotBeNull();
            response.ShouldBeEquivalentTo(_expectedAccount);
        }
    }
}
