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
    public class WhenGettingAnEmployerAgreement : ApiClientTestBase
    {
        private string _uri;
        private string _hashedAccountId = "AA";
        private string _hashedlegalEntityId = "BB";
        private string _hashedAgreementId = "CC";

        public override void HttpClientSetup()
        {
            _uri = $"/api/accounts/{_hashedAccountId}/legalEntities/{_hashedlegalEntityId}/agreements/{_hashedAgreementId}/agreement";
            var absoluteUri = Configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            var agreement = new EmployerAgreementView { HashedAccountId = _hashedAccountId };

            HttpClient.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(agreement)));
        }

        [Test]
        public async Task ThenTheCorrectUrlIsCalled()
        {
            //Act
            var response = await ApiClient.GetEmployerAgreement(_hashedAccountId, _hashedlegalEntityId, _hashedAgreementId);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(_hashedAccountId,response.HashedAccountId);
        }
    }
}
