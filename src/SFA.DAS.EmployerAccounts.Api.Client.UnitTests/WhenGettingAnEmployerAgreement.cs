﻿using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Api.Client.UnitTests
{
    public class WhenGettingAnEmployerAgreement : ApiClientTestBase
    {
        private const string HashedAccountId = "ABC123";
        private const string HashedlegalEntityId = "DEF456";
        private const string HashedAgreementId = "GHI789";

        private string _uri;

        public override void HttpClientSetup()
        {
            _uri = $"/api/accounts/{HashedAccountId}/legalEntities/{HashedlegalEntityId}/agreements/{HashedAgreementId}";
            var absoluteUri = Configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            var agreement = new EmployerAgreementView { HashedAccountId = HashedAccountId };

            HttpClient.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(agreement)));
        }

        [Test]
        public async Task ThenTheCorrectUrlIsCalled()
        {
            //Act
            var response = await ApiClient.GetEmployerAgreement(HashedAccountId, HashedlegalEntityId, HashedAgreementId);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(HashedAccountId,response.HashedAccountId);
        }
    }
}
