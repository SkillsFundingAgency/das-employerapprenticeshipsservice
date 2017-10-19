using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingLegalEntitiesForAnAccount : ApiClientTestBase
    {
        private string _uri;
        private List<ResourceViewModel> _legalEntities;

        public override void HttpClientSetup()
        {
            _uri = $"/api/accounts/{TextualAccountId}/legalentities";
            var absoluteUri = Configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            _legalEntities = new List<ResourceViewModel>() { new ResourceViewModel { Id = "1", Href = "/api/legalentities/test1" } };
            
            HttpClient.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_legalEntities)));
        }

        [Test]
        public async Task ThenTheLegalEntitiesAreReturned()
        {
            // Act
            var response = await ApiClient.GetLegalEntitiesConnectedToAccount(TextualAccountId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<ResourceList>(response);
            response.Should().NotBeNull();
            response.ShouldBeEquivalentTo(_legalEntities);
        }
    }
}
