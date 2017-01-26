using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingALegalEntity
    {
        private AccountApiConfiguration _configuration;
        private Mock<SecureHttpClient> _httpClient;
        private AccountApiClient _apiClient;
        private LegalEntityViewModel _expectedLegalEntity;
        private string _uri;

        [SetUp]
        public void Arrange()
        {
            _configuration = new AccountApiConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };

            _uri = "http://localhost/api/accounts/ABC123/legalentities/123";
            _expectedLegalEntity = new LegalEntityViewModel
            {
                Id = 123,
                Code = "Code",
                Name = "Name",
                DateOfIncorporation = DateTime.Now.AddYears(-1),
                Source = "Source",
                RegisteredAddress = "An address",
                CompanyStatus = "Status"
            };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(_uri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_expectedLegalEntity)));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenTheLegalEntityIsReturned()
        {
            // Act
            var response = await _apiClient.GetResource<LegalEntityViewModel>(_uri);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<LegalEntityViewModel>(response);
            response.Should().NotBeNull();
            response.ShouldBeEquivalentTo(_expectedLegalEntity);
        }
    }
}
