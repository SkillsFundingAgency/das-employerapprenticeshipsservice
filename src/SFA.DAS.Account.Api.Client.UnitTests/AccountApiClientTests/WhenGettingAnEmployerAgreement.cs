using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAnEmployerAgreement
    {
        private AccountApiConfiguration _configuration;
        private string _uri;
        private string _hashedAccountId;
        private string _hashedlegalEntityId;
        private string _hashedAgreementId;
        private Mock<SecureHttpClient> _httpClient;
        private AccountApiClient _apiClient;

        [SetUp]
        public void Arrange()
        {
            _configuration = new AccountApiConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };

            _hashedAccountId = "123ABC";
            _hashedlegalEntityId = "TRG567";
            _hashedAgreementId = "SDF678";

            _uri = $"/api/accounts/{_hashedAccountId}/legalEntities/{_hashedlegalEntityId}/agreements/{_hashedAgreementId}/agreement";
            var absoluteUri = _configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            var agreement = new EmployerAgreementView {HashedAccountId = _hashedAccountId};

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(agreement)));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }

        [Test]
        public async Task ThenTheCorrectUrlIsCalled()
        {
            //Act
            var response = await _apiClient.GetEmployerAgreement(_hashedAccountId, _hashedlegalEntityId, _hashedAgreementId);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(_hashedAccountId,response.HashedAccountId);
        }
    }
}
