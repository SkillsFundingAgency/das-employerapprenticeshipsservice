using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAPayeScheme
    {
        private AccountApiConfiguration _configuration;
        private Mock<SecureHttpClient> _httpClient;
        private AccountApiClient _apiClient;
        private PayeSchemeViewModel _expectedPayeScheme;
        private string _uri;

        [SetUp]
        public void Arrange()
        {
            _configuration = new AccountApiConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };

            _uri = "http://localhost/api/accounts/ABC123/payeschemes/ABC%f123";
            _expectedPayeScheme = new PayeSchemeViewModel
            {
                Ref = "ABC/123",
                Name = "Name"
            };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(_uri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_expectedPayeScheme)));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenTheLegalEntityIsReturned()
        {
            // Act
            var response = await _apiClient.GetResource<PayeSchemeViewModel>(_uri);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<PayeSchemeViewModel>(response);
            response.Should().NotBeNull();
            response.ShouldBeEquivalentTo(_expectedPayeScheme);
        }
    }
}
