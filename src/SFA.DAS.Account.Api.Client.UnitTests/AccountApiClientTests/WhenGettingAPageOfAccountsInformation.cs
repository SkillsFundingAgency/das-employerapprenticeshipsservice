using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client.Dtos;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAPageOfAccountsInformation
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
                    JsonConvert.SerializeObject(new Dtos.PagedApiResponseViewModel<AccountInformationViewModel>
                    {
                        Page = 1,
                        TotalPages = 1,
                        Data = new List<AccountInformationViewModel>
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
                        }
                    })));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }
        [Test]
        public async Task ThenItShouldCallTheApiWithTheCorrectUrl()
        {
            //Arrange
            var dateFrom = new DateTime(2016,10,01);
            var dateto = new DateTime(2017, 11, 12);
            var pageNumber = 2;
            var pageSize = 100;

            // Act
            await _apiClient.GetPageOfAccountInformation(dateFrom, dateto, pageNumber, pageSize);

            // Assert
            var expectedUrl = $"http://some-url/api/accountsinformation?fromDate=2016-10-01&toDate=2017-11-12&page={pageNumber}&pageSize={pageSize}";
            _httpClient.Verify(c => c.GetAsync(expectedUrl), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnAPageOfAccounts()
        {
            // Act
            var actual = await _apiClient.GetPageOfAccountInformation(new DateTime(2016, 10, 01), new DateTime(2016, 11, 16));

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldDeserializeTheResponseCorrectly()
        {
            // Act
            var actual = await _apiClient.GetPageOfAccountInformation(new DateTime(2016, 10, 01), new DateTime(2016, 11, 16));

            // Assert
            Assert.IsAssignableFrom<Dtos.PagedApiResponseViewModel<Dtos.AccountInformationViewModel>>(actual);
        }
    }
}
