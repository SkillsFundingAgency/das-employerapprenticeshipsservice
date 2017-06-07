using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAccountDetailsByInternalId
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

            _uri = "/api/accounts/internal/123";
            var absoluteUri = _configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            _expectedAccount = new AccountDetailViewModel
            {
                AccountId = 123,
                HashedAccountId = "1",
                DasAccountName = "Account 1",
                DateRegistered = DateTime.Now.AddYears(-1),
                OwnerEmail = "test@email.com",
                LegalEntities = new ResourceList(new[] { new ResourceViewModel { Id = "1", Href = "/api/legalentities/test1" } }),
                PayeSchemes = new ResourceList(new[] { new ResourceViewModel { Id = "1", Href = "/api/payeschemes/test1" } })
            };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_expectedAccount)));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }
        

        [Test]
        public async Task ThenTheCorrectEndpointIsCalled()
        {
            //Act
            var actual = await _apiClient.GetAccount(123);

            //Assert
            Assert.IsNotNull(actual);
        }
    }
}