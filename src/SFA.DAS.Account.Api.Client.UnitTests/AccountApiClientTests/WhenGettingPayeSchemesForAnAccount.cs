using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingPayeSchemesForAnAccount
    {
        private AccountApiConfiguration _configuration;
        private Mock<SecureHttpClient> _httpClient;
        private AccountApiClient _apiClient;
        private string _uri;
        private List<ResourceViewModel> _payeSchemes;

        [SetUp]
        public void Arrange()
        {
            _configuration = new AccountApiConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };

            _uri = "/api/accounts/ABC123/payeschemes";
            var absoluteUri = _configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            _payeSchemes = new List<ResourceViewModel>() { new ResourceViewModel { Id = "1", Href = "/api/payeschemes/test1" } };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_payeSchemes)));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenTheAccountDetailsAreReturned()
        {
            // Act
            var response = await _apiClient.GetPayeSchemesConnectedToAccount("ABC123");

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<ResourceList>(response);
            response.Should().NotBeNull();
            response.ShouldBeEquivalentTo(_payeSchemes);
        }
    }
}
