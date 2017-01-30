using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client.Dtos;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAccountsInformationById
    {
        private AccountApiConfiguration _configuration;
        private Mock<SecureHttpClient> _httpClient;
        private AccountApiClient _apiClient;

        [SetUp]
        public void Arrange()
        {
            _configuration = new AccountApiConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    JsonConvert.SerializeObject(new List<AccountInformationViewModel>
                    {
                        new AccountInformationViewModel
                        {
                            DasAccountName = "Test Account",
                            OrganisationName = "Test Organisation",
                            OrganisationStatus = "Active",
                            DasAccountId = "ABC",
                            DateRegistered = DateTime.Now,
                            OrganisationId = 123,
                            OrganisationNumber = "12345",
                            OrganisationRegisteredAddress = "Test Address",
                            OrganisationSource = "Source",
                            OrgansiationCreatedDate = DateTime.Now.AddDays(-31),
                            OwnerEmail = "test@email.com",
                            PayeSchemeName = "Scheme"
                        }
                    })));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }
        [Test]
        public async Task ThenItShouldCallTheApiWithTheCorrectUrl()
        {
            //Arrange
            var accountId = "ABC123";

            // Act
            await _apiClient.GetAccountInformationById(accountId);

            // Assert
            var expectedUrl = $"http://some-url/api/accountsinformation/{accountId}";
            _httpClient.Verify(c => c.GetAsync(expectedUrl), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnAccountsInformation()
        {
            // Act
            var actual = await _apiClient.GetAccountInformationById("ABC123");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldDeserializeTheResponseCorrectly()
        {
            // Act
            var actual = await _apiClient.GetAccountInformationById("ABC123");

            // Assert
            Assert.IsAssignableFrom<List<AccountInformationViewModel>>(actual);
        }
    }
}
