using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingLegalEntityDetailsForAnAccount : ApiClientTestBase
    {
        private List<LegalEntityViewModel> _legalEntities;
        private string _uri;

        public override void HttpClientSetup()
        {
            _uri = $"/api/accounts/{TextualAccountId}/legalentities?includeDetails=true";
            var absoluteUri = Configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            _legalEntities = new List<LegalEntityViewModel> { new LegalEntityViewModel { AccountLegalEntityId = 1} };
            
            HttpClient.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_legalEntities)));
        }
        
        [Test]
        public async Task ThenTheLegalEntitiesAreReturned()
        {
            // Act
            var response = await ApiClient.GetLegalEntityDetailsConnectedToAccount(TextualAccountId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<List<LegalEntityViewModel>>(response);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(_legalEntities);
        }
    }
}