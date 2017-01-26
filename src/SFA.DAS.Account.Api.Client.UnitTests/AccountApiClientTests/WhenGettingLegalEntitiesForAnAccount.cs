using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client.Dtos;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingLegalEntitiesForAnAccount
    {
        private AccountApiConfiguration _configuration;
        private Mock<SecureHttpClient> _httpClient;
        private AccountApiClient _apiClient;
        private string _uri;
        private List<ResourceViewModel> _legalEntities;

        [SetUp]
        public void Arrange()
        {
            _configuration = new AccountApiConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };

            _uri = "http://localhost/api/accounts/ABC123/legalentities";
            _legalEntities = new List<ResourceViewModel>() { new ResourceViewModel { Id = "1", Href = "/api/legalentities/test1" } };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(_uri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_legalEntities)));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenTheAccountDetailsAreReturned()
        {
            // Act
            var response = await _apiClient.GetResource<List<ResourceViewModel>>(_uri);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<List<ResourceViewModel>>(response);
            response.Should().NotBeNull();
            response.ShouldBeEquivalentTo(_legalEntities);
        }
    }
}
